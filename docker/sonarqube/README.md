# SonarQube Setup

```
sudo sysctl -w vm.max_map_count=262144
```

```
docker-compose --f docker-compose-sonar.yml pull
docker-compose --f docker-compose-sonar.yml up
```

L: admin
P: admin

AzureDevops
05f9e3397c26f79ed118df0a91a109d6a9c34b54


https://gist.github.com/ThabetAmer/b818a262c71467c7403fd75bc526b8f9

dotnet tool install --global dotnet-sonarscanner


### Run sonarscanner
```
dotnet sonarscanner begin /k:"AzureDevopsKats" /d:sonar.host.url="http://notebooks:9000"  /d:sonar.login="05f9e3397c26f79ed118df0a91a109d6a9c34b54"

dotnet build

dotnet sonarscanner end /d:sonar.login="05f9e3397c26f79ed118df0a91a109d6a9c34b54"
```