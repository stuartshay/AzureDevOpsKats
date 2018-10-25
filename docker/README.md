
### Docker 

```
cd AzureDevOpsKats
docker build -f docker/azuredevopskats-web-local.dockerfile/Dockerfile  -t stuartshay/azuredevopskats  .
docker push stuartshay/azuredevopskats:latest  
```