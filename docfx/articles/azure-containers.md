## Azure Containers

https://docs.microsoft.com/en-us/azure/container-instances/container-instances-managed-identity

### Create Shell Variables

```bash
resourceGroup="AzureDevOpsKats-RG"
containerName="devopskats"
```

### Exec Container

```bash
az container exec \
  --resource-group $resourceGroup \
  --name $containerName \
  --exec-command "/bin/bash"
```

### Get Secrets

Get Access Token

```bash
token=$(curl 'http://169.254.169.254/metadata/identity/oauth2/token?api-version=2018-02-01&resource=https%3A%2F%2Fvault.azure.net' -H Metadata:true | jq -r '.access_token')

echo $token
```

Authenticate and Get Secret

```bash
curl https://devopskatskeyvault.vault.azure.net/secrets/AzureDevopsConnectionString/?api-version=2016-10-01 -H "Authorization: Bearer $token"
```
