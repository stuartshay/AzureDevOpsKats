1. Create a resource group that serves as the container for the deployed resources.

```
az group create --name AzureDevOpsKatsGroup --location "Central US"
```

2. Deploy to the resource group the template that defines the resources to create


* AppService

```
az group deployment create \
  --name AppServiceARMDeployment \
  --resource-group AzureDevOpsKatsGroup \
  --template-file arm_release/AppServiceARMTemplate.json \
  --parameters "@arm_release/AppServiceARM.parameters.json"
```

* WebSite

```
az group deployment create \
  --name WebSitesARMDeployment \
  --resource-group AzureDevOpsKatsGroup \
  --template-file arm_release/WebSitesARMTemplate.json \
  --parameters "@arm_release/WebSitesARMTemplate.parameters.json"
```

* Storage Account & Blob Storage

```
az group deployment create \
  --name StorageAccountARMDeployment \
  --resource-group AzureDevOpsKatsGroup \
  --template-file arm_release/storageAccountTemplate.json \
  --parameters "@arm_release/storageAccountTemplate.parameters.json"
```

* Key Vault

```
az group deployment create \
  --name KeyVaultARMDeployment \
  --resource-group AzureDevOpsKatsGroup \
  --template-file arm_release/KeyVaultTemplate.json \
  --parameters "@arm_release/KeyVaultTemplate.parameters.json"
```