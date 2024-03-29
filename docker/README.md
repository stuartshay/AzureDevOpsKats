## Docker

Setup

Windows Host File: `C:\Windows\System32\drivers\etc\hosts`</br>
Mac/Linux: `/etc/hosts`

```
127.0.0.1 azuredevopskats-web
127.0.0.1 traefik
127.0.0.1 redis

127.0.0.1 es01
127.0.0.1 kib01
127.0.0.1 apm-server
```

Reload Windows Cache

```
ipconfig /flushdns
```

### Docker Compose

```
docker-compose --file docker-compose.yml pull
docker-compose --file docker-compose.yml up  --scale azuredevopskats-web=5
```

### Elastic

```
docker-compose --file docker-compose-elastic.yml up
docker-compose --file docker-compose-elastic.yml pull
```

Azure Devops Website

```
http://azuredevopskats-web
```

Kibana

```
http://es01:5601/app/home#/
```

Elastic Search Endpoint

```
http://es01:9200/app/home#/
```

Traefik

```
http://traefik:8080
```

### Scale Command

```
docker-compose --file docker-compose.yml up -d --scale <SERVICE>=<NUMBER>
```

## Linux Local Build

Docker Compose

```
cd AzureDevOpsKats
docker-compose -f docker/docker-compose-build.yml build
docker-compose -f docker/docker-compose-build.yml up
```

Docker

```
cd AzureDevOpsKats
docker build -f docker/azuredevopskats-web-multi.dockerfile/Dockerfile -t stuartshay/azuredevopskats  .
```

Kompose

```
kompose convert -f docker-compose-local.yml
```

### Reference

.NET Docker Images

```
https://github.com/atomist-container-images/mcr.microsoft.com-dotnet_aspnet/blob/main/Dockerfile
```
