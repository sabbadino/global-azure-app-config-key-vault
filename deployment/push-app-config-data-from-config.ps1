[CmdletBinding()]
param (
	[Parameter(Mandatory = $True)]
	[string] $appConfigName ,
	[Parameter(Mandatory = $True)]
	[string] $configPath,
	[bool] $removeMissingKeys = $False,
	[bool] $whatif = $False
)


function RemoveMissingKyes ([bool] $removeMissingKeys, [bool] $whatif) {
	Write-Host removing missing keys
	$keyList = $(az appconfig kv list --all --name $appConfigName) | ConvertFrom-Json
	foreach ($key in $keyList) {
		$keyName = $key.key
		Write-Host searching key $keyName
		$resSettings = $xmlSettings | Select-XML -XPath "/configuration/add[@key='$keyName']"
		if ($null -eq $resSettings) {
			if ($whatif -eq $False -and $removeMissingKeys -eq $True) {
				Write-Host "!!!!!!!! removing $keyName"
				#removing secrets
				$ret = az appconfig kv delete --name $appConfigName --key $keyName --yes
				if (!$?) {
					Write-Error "Error calling az appconfig kv delete"
					exit -1
				}
				
			}
			else {
				Write-Host "!!!!!!!! I would have removed $keyName"
			}
			
		}
		else {
				Write-Host "$keyName has not to be removed"
		}
	}
}

[xml]$xmlSettings = Get-Content -Path $configPath
	$xmlSettings.SelectNodes('/configuration/add') | ForEach-Object {
	$keyInxml = $_.key;
	write-host processing appsettings key $keyInxml
	$keyList = $(az appconfig kv list --name $appConfigName --key $keyInxml) | ConvertFrom-Json
	if (!$?) {
			Write-Error "Error calling az appconfig kv list"
			exit -1
	}
	if($keyList.length -ne 0) {
		$key = $keyList[0];
	}
	
	$valueInxml = $_.value;
	if($valueInxml -ceq "") {
		write-host valueInxml is null, converting to one space
		$valueInxml = " "
	}
	
	$valueInxmlCleaned = $valueInxml
	if($_.contentType -eq "application/json") {
		$valueInxmlCleaned = $valueInxml.replace('\','')
	}
	if($_.isKeyVaultRef -eq "true") {
		$secretIdentifier = $_.secretIdentifier
		$valueInxmlCleaned = "{`"uri`":`"$secretIdentifier`"}"
	}
	if ($key -ne $null -and $key.value -ceq $valueInxmlCleaned) {
		write-host value of key $keyInxml not changed. SKIPPING 
	}
	else {
		write-host value of key $keyInxml changed. CHANGED  : in app conf:  $key.value in config file : $valueInxmlCleaned
		if($whatif -eq $False) {
			write-Host adding/updating key $keyInxml 
			if($null -eq $_.contentType) {
				if($_.isKeyVaultRef -eq "true") {
					$secretIdentifier = $_.secretIdentifier
					Write-Host "!!!!!!!! adding/updating key $keyInxml as key vault ref $secretIdentifier "
					az appconfig kv set-keyvault  --name $appConfigName --key  $keyInxml --secret-identifier $secretIdentifier  --yes	
					$somethingChanged=$True
				}
				else {
					Write-Host "!!!!!!!! adding/updating key $keyInxml with value: $valueInxml with no content type"
					az appconfig kv set  --name $appConfigName --key  $keyInxml --value "$valueInxml" --yes
					$somethingChanged=$True
				}
			}
			else {
				$contentType= $_.contentType
				Write-Host "!!!!!!!! adding/updating key $keyInxml with value: $valueInxml and content type $contentType"
				az appconfig kv set  --name $appConfigName --key  $keyInxml --value "$valueInxml" --yes --content-type $contentType
				$somethingChanged=$True
			}
			if (!$?) {
				Write-Error "Error calling az appconfig kv set"
				exit -1
			}
		}
		else {
			write-Host "!!!!!!!! I would have added/updated key $keyInxml"
			Write-Verbose "!!!!!!!! I would have added/updated key $keyInxml with value: $valueInxml"
		}
		
	}
} 

# removing settings that are not present in the config
Write-Host RemoveMissingKyes $removeMissingKeys
RemoveMissingKyes $removeMissingKeys $whatif

# i could keep track if something has chnaged and do not chnage sentinel value to triggere refresh, but I don't know if akv values that are referenced have been changed 
Write-Host UPDATING Sentinel Key
az appconfig kv set  --name $appConfigName --key  "Sentinel" --value (Get-Date -Format yyyy-MM-dd-HH:mm:ss.fff) --yes


