#az login
az account set --subscription "d642528f-3525-4e1b-bf17-fe100458b4e1"
az keyvault key create --vault-name msc-key-vault-session --name onemorekey --size 2048 