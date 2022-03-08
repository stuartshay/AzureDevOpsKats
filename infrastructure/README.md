## AzureDevOpsKats CLI Script Deployment

### Create Shell Variables

```bash
resourceGroup=AzureDevOpsKats-RG
location="eastus"
dnsNameLabel="azuredevopskats"
containerName="devopskats"
dockerImage="stuartshay/azuredevopskats:latest"
storageAccount="azurekatsimages01"
shareName="devopskatsimages"
keyVaultName="devopskatskeyVault"
```

Turn on persisted parameter

```bash
az config param-persist on
```

### Resource Group

Create a resource group that serves as the container for the deployed resources.

```bash
az group create --name $resourceGroup --location $location
```

### Storage Account

```bash
az storage account create --resource-group $resourceGroup \
        --name $storageAccount \
        --location $location \
        --sku Standard_LRS
```

File Share

```bash
az storage share create \
  --name $shareName \
  --account-name $storageAccount
```

### Create Key Vault

```bash
az keyvault create --name $keyVaultName --resource-group $resourceGroup --location $location
```

Create Secret

```bash
az keyvault secret set --vault-name $keyVaultName --name "AzureDevopsConnectionString" --value "db='localhost:username:password'"
```

## Container instance

https://docs.microsoft.com/en-us/cli/azure/container?view=azure-cli-latest

Get Storage Account Key

```
STORAGE_KEY=$(az storage account keys list --resource-group $resourceGroup \
--account-name $storageAccount --query "[0].value" --output tsv)

echo $STORAGE_KEY
```

Create Container

```bash
az container create --resource-group $resourceGroup \
      --name $containerName \
      --image $dockerImage \
      --dns-name-label $dnsNameLabel \
      --azure-file-volume-account-name $storageAccount \
      --azure-file-volume-account-key $STORAGE_KEY \
      --azure-file-volume-share-name $shareName \
      --azure-file-volume-mount-path /images \
      --environment-variables ASPNETCORE_ENVIRONMENT=AzureContainer key2=value2 \
      --ports 5000
```

### Attach output streams

```bash
az container attach --resource-group $resourceGroup --name $containerName
```

### Cleanup

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
