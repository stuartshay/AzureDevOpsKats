## Azure Container Registry

https://docs.microsoft.com/en-us/azure/container-registry/container-registry-get-started-azure-cli

### Set Shell Variables

```bash
resourceGroup="AzureDevOpsKats-RG"
containerRegistry="AzureDevOpsKats"
```

### Create a container registry

```bash
az acr create --resource-group $resourceGroup \
  --name $containerRegistry --sku Basic
```

### Create Service Prinicipal

Get the GroupId of the Resource Group

```bash
groupId=$(az group show --name $resourceGroup \
  --query id --output tsv)

echo $groupId
```

Create the service principal:

```bash
az ad sp create-for-rbac --scope $groupId \
  --role Contributor \
  --sdk-auth
```

Save the Json

```bash
{
  "clientId": "xxxx6ddc-xxxx-xxxx-xxx-ef78a99dxxxx",
  "clientSecret": "xxxx79dc-xxxx-xxxx-xxxx-aaaaaec5xxxx",
  "subscriptionId": "xxxx251c-xxxx-xxxx-xxxx-bf99a306xxxx",
  "tenantId": "xxxx88bf-xxxx-xxxx-xxxx-2d7cd011xxxx",
  "activeDirectoryEndpointUrl": "https://login.microsoftonline.com",
  "resourceManagerEndpointUrl": "https://management.azure.com/",
  "activeDirectoryGraphResourceId": "https://graph.windows.net/",
  "sqlManagementEndpointUrl": "https://management.core.windows.net:8443/",
  "galleryEndpointUrl": "https://gallery.azure.com/",
  "managementEndpointUrl": "https://management.core.windows.net/"
}
```

Update service principal for registry authentication

```bash
registryId=$(az acr show --name $containerRegistry \
  --query id --output tsv)

echo $registryId
```

Assign the Role to the Container Registry

```bash
az role assignment create --assignee <ClientId> \
  --scope $registryId \
  --role AcrPush
```

### Create Git Hub Secrets

| Secret                | Value                                                                                  |
| --------------------- | -------------------------------------------------------------------------------------- |
| AZURE_CREDENTIALS     | The entire JSON output from the service principal creation step                        |
| REGISTRY_LOGIN_SERVER | The login server name of your registry (all lowercase). Example: myregistry.azurecr.io |
| REGISTRY_USERNAME     | The clientId from the JSON output from the service principal creation                  |
| REGISTRY_PASSWORD     | The clientSecret from the JSON output from the service principal creation              |
| RESOURCE_GROUP        | The name of the resource group you used to scope the service principal                 |
