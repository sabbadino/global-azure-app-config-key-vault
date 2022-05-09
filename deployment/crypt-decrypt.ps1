[CmdletBinding()]
param (
[Parameter(Mandatory = $True)]
    [string] $keyvaultname ,
	[Parameter(Mandatory = $True)]
    [string] $value 
	)
$x= az keyvault key encrypt --name KeyForSecrets --vault-name $keyvaultname --value $value --data-type plaintext --algorithm RSA-OAEP | ConvertFrom-Json
write-host "encripted "  $x.result
# only allowed princiapls can do this 
$y= az keyvault key decrypt --name KeyForSecrets --vault-name $keyvaultname --value $x.result --data-type plaintext --algorithm RSA-OAEP | ConvertFrom-Json
write-host "decripted "  $y.result
