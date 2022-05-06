az cloud set --name AzureCloud
az login 
az ad sp create-for-rbac --name myServicePrincipalName3 --role Contributor  --scopes /subscriptions/2d94b8b8-a8cc-482f-8fa5-83b0d5c96312
