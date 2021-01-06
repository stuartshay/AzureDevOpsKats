
## Docker 

Setup

Windows Host File:  ```C:\Windows\System32\drivers\etc\hosts```    
Mac/Linux: ```/etc/hosts```

```
127.0.0.1 azuredevopskats-web
```

### Docker Compose 

```
docker-compose pull
docker-compose up
docker-compose --scale azuredevopskats-web=5
```

```
http://azuredevopskats-web
```

### Scale Command
```
docker-compose --file docker-compose.yml up -d --scale <SERVICE>=<NUMBER> 
```


## Linux Local Build
```
cd AzureDevOpsKats
docker build -f docker/azuredevopskats-web-multi.dockerfile/Dockerfile -t stuartshay/azuredevopskats  .
  ```

