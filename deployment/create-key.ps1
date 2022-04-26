[CmdletBinding()]
param (
	[Parameter(Mandatory = $True)]
    [string] $keyvaultname
	)
az keyvault key create --vault-name "$keyvaultname" -n KeyForSecrets --protection software