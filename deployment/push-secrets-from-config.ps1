[CmdletBinding()]
param (
	[Parameter(Mandatory = $True)]
	[string] $keyVaultName ,
	[Parameter(Mandatory = $True)]
	[string] $configPath,
	[bool] $removeMissingSecrets = $False,
	[bool] $whatif = $False
)

function RemoveMissingSecrets ([bool] $removeMissingSecrets, [bool] $whatif) {
	Write-Host removing missing secrets
	$secretList = $(az keyvault secret list --vault-name $keyVaultName) | ConvertFrom-Json
	foreach ($secret in $secretList) {
		Write-Host searching $secret.name
		$secretName = $secret.name
		$resSettings = $xmlSecrets | Select-XML -XPath "/configuration/add[@key='$secretName']"
		if ($null -eq $resSettings) {
			if ($whatif -eq $False -and $removeMissingSecrets -eq $True) {
				Write-Host "!!!!!!!! removing $secretName"
				#removing secrets
				$ret = az keyvault secret delete --vault-name $keyVaultName --name $secretName
				if (!$?) {
					Write-Error "Error calling az keyvault secret delete"
					exit -1
				}
				$ret = az keyvault secret purge --vault-name $keyVaultName --name $secretName
				if (!$?) {
					Write-Error "Error calling az keyvault secret purge"
					exit -1
				}
			}
			else {
				Write-Host "!!!!!!!! I would have removed $secretName"
			}
			
		}
	}
}

function DecryptIfRequired () {
	if($_.isEncrypted -ceq "true" -or $configurationNode.isEncrypted -ceq "true") {
		write-host value for $keyInxml is encrypted with value $valueInxml
		$y= az keyvault key decrypt --name KeyForSecrets --vault-name $keyVaultName --value $valueInxml --data-type plaintext --algorithm RSA-OAEP | ConvertFrom-Json
		if (!$?) {
				Write-Error "Error calling az keyvault key decrypt"
				exit -1
		}
		$debugvalue = $y.result
		 Write-Verbose "decripted value: $debugvalue"
		return $y.result
	}
	else {
		write-host value for $keyInxml is not encrypted, returning the value as it is
		return $valueInxml
	}
}


[xml]$xmlSecrets = Get-Content -Path $configPath
$configurationNode = $xmlSecrets.SelectSingleNode('/configuration')
$xmlSecrets.SelectNodes('/configuration/add') | ForEach-Object {
	$keyInxml = $_.key;
	write-host processing appsettings key $keyInxml
	# secret show give error is secret not present 
	$query = "[? name=='" + $keyInxml + "']"
	$secretList = $(az keyvault secret list --vault-name $keyVaultName --query $query) | ConvertFrom-Json
	if (!$?) {
			Write-Error "Error calling az keyvault secret list"
			exit -1
	}
	$secret = $null;
	
	$valueInxml = $_.value;
	$valueInxml = DecryptIfRequired
	 Write-Verbose "valueInxml after DecryptIfRequired: $valueInxml"
	if($valueInxml -ceq "") {
		write-host valueInxml is null, converting to one space
		$valueInxml = " "
	}
	
	if ($secretList.Count -ne 0) {
		$secret = az keyvault secret show --vault-name $keyVaultName --name $keyInxml | ConvertFrom-Json
		if (!$?) {
			Write-Error "Error calling az keyvault secret show"
			exit -1
		}
	}
	if ($secret -ne $null -and $secret.value -ceq $valueInxml) {
		write-host value of key $keyInxml not changed. SKIPPING 
	}
	else {
		if($whatif -eq $False) {
			write-Host adding/updating key $keyInxml
			Write-Verbose "!!!!!!!! adding/updating key $keyInxml with value: $valueInxml"
			$ret = az keyvault secret set --vault-name $keyVaultName --name $keyInxml --value "$valueInxml"
			if (!$?) {
				Write-Error "Error calling az keyvault secret set"
				exit -1
			}
		}
		else {
			write-Host "!!!!!!!! I would have added/updated key $keyInxml"
			Write-Verbose "!!!!!!!! I would have added/updated key $keyInxml with value: $valueInxml"
		}
		
	}
} 
 
# removing secrets that are not present in the config
Write-Host removeMissingSecrets $removeMissingSecrets
RemoveMissingSecrets $removeMissingSecrets $whatif

