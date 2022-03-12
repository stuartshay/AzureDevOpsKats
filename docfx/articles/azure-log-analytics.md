## Azure Log Analytics

![](assets/azure-log-analytics.png)

### Container Instance Logs

```bash
ContainerInstanceLog_CL
| where ContainerName_s == "azuredevopskats-container"
```
