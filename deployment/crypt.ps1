[CmdletBinding()]
param (
	[Parameter(Mandatory = $True)]
    [string] $secret 
	)
#az login
az account set --subscription "d642528f-3525-4e1b-bf17-fe100458b4e1"
# anyone can do this .. 
$x= az keyvault key encrypt --name mykey1 --vault-name msc-key-vault-session --value $secret --data-type base64 --algorithm RSA-OAEP | ConvertFrom-Json
write-host "encripted "  $x.result
# only allowed princiapls can do this 
$y= az keyvault key decrypt --name mykey1 --vault-name msc-key-vault-session --value $x.result --data-type base64 --algorithm RSA-OAEP | ConvertFrom-Json
write-host "decripted "  $y.result
