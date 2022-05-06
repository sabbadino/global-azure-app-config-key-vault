ping global-azure-2022-app-configuration.azconfig.io
az login --service-principal -u ccd724c0-d336-411a-a69d-3488cd67091e -p mRE7Q~swLNYS18eKoPN9gNLbo_zDGw1b05hgL --tenant 088e9b00-ffd0-458e-bfa1-acf4c596d3cb
write-host before sub set 
az account set --subscription 8d6c7fe3-640b-487e-9739-0973be82ddeb
write-host before appconfig list
az appconfig list
write-host before appconfig kv list 
az appconfig kv list --name global-azure-2022-app-configuration
