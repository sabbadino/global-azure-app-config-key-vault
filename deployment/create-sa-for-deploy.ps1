az cloud set --name AzureCloud
az login 
az ad sp create-for-rbac --name myServicePrincipalName --role owner --scopes /subscriptions/2d94b8b8-a8cc-482f-8fa5-83b0d5c96312



#{
  #"appId": "8246d0ca-4731-4d14-a654-7fe542af48c8",
  #"displayName": "myServicePrincipalName",
  #"password": "HOwy-EW-SigYE4_Zn6RucogF2gJSWn4R6m",
  #"tenant": "15998043-4fae-445c-9cc7-3497b278212a"
#}