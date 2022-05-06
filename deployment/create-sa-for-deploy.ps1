az cloud set --name AzureCloud
az login 
az ad sp create-for-rbac --name myServicePrincipalName3 --role Contributor  --scopes /subscriptions/2d94b8b8-a8cc-482f-8fa5-83b0d5c96312


# subscription 2d94b8b8-a8cc-482f-8fa5-83b0d5c96312
#Azuresubscription1
#myServicePrincipalName
#{
  #"appId": "8246d0ca-4731-4d14-a654-7fe542af48c8",
  #"displayName": "myServicePrincipalName",
  #"password": "HOwy-EW-SigYE4_Zn6RucogF2gJSWn4R6m",
  #"tenant": "15998043-4fae-445c-9cc7-3497b278212a"
#}
# myServicePrincipalName2
#{
 # "appId": "41090a6f-fba7-4db5-9117-e79387950ef4",
  #"displayName": "myServicePrincipalName2",
  #"password": "N.0mkO2AjAzdb~vuL0MCeo-eCfhJ8-mXJK",
  #"tenant": "15998043-4fae-445c-9cc7-3497b278212a"
#}
#myServicePrincipalName3
#{
 # "appId": "609282a9-5edb-41c4-b679-b81029ae6549",
  #"displayName": "myServicePrincipalName3",
  #"password": "No4yBxVdN7S9Ktg-MoOg8LQFlnNnFpN5nD",
  #"tenant": "15998043-4fae-445c-9cc7-3497b278212a"
#}