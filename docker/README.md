
### Docker 


#### Linux Local Build
```
cd AzureDevOpsKats
docker build -f docker/azuredevopskats-web-multi.dockerfile/Dockerfile -t stuartshay/azuredevopskats  .
docker push stuartshay/azuredevopskats:latest  
```

#### Windows Local Build
```
cd AzureDevOpsKats
docker build -f docker/azuredevopskats-web-windows.dockerfile/Dockerfile -t stuartshay/azuredevopskats:win  .
docker push stuartshay/azuredevopskats:win

# Run Container
docker run -it -p 5000:5000 stuartshay/azuredevopskats:win

# Container IP
docker inspect -f "{{ .NetworkSettings.Networks.nat.IPAddress }}" <CONTAINER_ID>
```

