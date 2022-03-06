## AzureDevOps Kats Azure CLI Script Deployment

### Create Shell Variables

```
resourceGroup=AzureDevOpsKats-RG
location="eastus"
dnsNameLabel="azuredevopskats"
containerName="devopskats"
dockerImage="stuartshay/azuredevopskats:latest"
```

1. Create a resource group that serves as the container for the deployed resources.

```
az group create --name $resourceGroup --location $location
```

## Container instance

https://docs.microsoft.com/en-us/cli/azure/container?view=azure-cli-latest

```
az container create --resource-group $resourceGroup --name $containerName --image stuartshay/azuredevopskats:latest --dns-name-label $dnsNameLabel --ports 5000
```

### Attach output streams

```
az container attach --resource-group $resourceGroup --name $containerName
```

Cleanup

```
az container delete --resource-group $resourceGroup --name $containerName
```

## AppService

2. Deploy to the resource group the template that defines the resources to create

- AppService

```
az group deployment create \
  --name AppServiceARMDeployment \
  --resource-group AzureDevOpsKats-RG \
  --template-file arm_release/AppServiceARMTemplate.json \
  --parameters "@arm_release/AppServiceARM.parameters.json"
```

- WebSite

```
az group deployment create \
  --name WebSitesARMDeployment \
  --resource-group AzureDevOpsKats-RG \
  --template-file arm_release/WebSitesARMTemplate.json \
  --parameters "@arm_release/WebSitesARMTemplate.parameters.json"
```

- Storage Account & Blob Storage

```
az group deployment create \
  --name StorageAccountARMDeployment \
  --resource-group AzureDevOpsKats-RG \
  --template-file arm_release/StorageAccountTemplate.json \
  --parameters "@arm_release/StorageAccountTemplate.parameters.json"
```

- Key Vault

```
az group deployment create \
  --name KeyVaultARMDeployment \
  --resource-group AzureDevOpsKats-RG \
  --template-file arm_release/KeyVaultTemplate.json \
  --parameters "@arm_release/KeyVaultTemplate.parameters.json"
```

- Create & Deploy Azure Container Services

```
az ad sp create-for-rbac --role="Contributor" \
  --scopes="/subscriptions/4ffc998e-322d-4b70-9e93-1515eed562c6/resourceGroups/AzureDevOpsKatsGroup"
```

```
az group deployment create \
  --name ContainerServiceARMDeployment \
  --resource-group AzureDevOpsKatsGroup \
  --template-file arm_release/ContainerServiceARMTemplate.json \
  --parameters "@arm_release/ContainerServiceARMTemplate.parameters.json"
```
