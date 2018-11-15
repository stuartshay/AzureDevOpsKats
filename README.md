# AzureDevOpsKats

[![SonarCloud](http://sonar.navigatorglass.com:9000/api/project_badges/measure?project=9c944632fe7a37d24b533680dac1e45b5b34fea7&metric=alert_status)](http://sonar.navigatorglass.com:9000/dashboard?id=9c944632fe7a37d24b533680dac1e45b5b34fea7)
[![SonarCloud](http://sonar.navigatorglass.com:9000/api/project_badges/measure?project=9c944632fe7a37d24b533680dac1e45b5b34fea7&metric=reliability_rating)](http://sonar.navigatorglass.com:9000/dashboard?id=9c944632fe7a37d24b533680dac1e45b5b34fea7)
[![SonarCloud](http://sonar.navigatorglass.com:9000/api/project_badges/measure?project=9c944632fe7a37d24b533680dac1e45b5b34fea7&metric=security_rating)](http://sonar.navigatorglass.com:9000/dashboard?id=9c944632fe7a37d24b533680dac1e45b5b34fea7)
[![SonarCloud](http://sonar.navigatorglass.com:9000/api/project_badges/measure?project=9c944632fe7a37d24b533680dac1e45b5b34fea7&metric=sqale_rating)](http://sonar.navigatorglass.com:9000/dashboard?id=9c944632fe7a37d24b533680dac1e45b5b34fea7)


[![This image on DockerHub](https://img.shields.io/docker/pulls/stuartshay/azuredevopskats.svg)](https://hub.docker.com/r/stuartshay/azuredevopskats/)
[![](https://images.microbadger.com/badges/version/stuartshay/azuredevopskats:2.1.1-base.svg)](https://microbadger.com/images/stuartshay/azuredevopskats:2.1.1-base "microbadger.com")
[![](https://images.microbadger.com/badges/version/stuartshay/azuredevopskats:2.1.9-build.svg)](https://microbadger.com/images/stuartshay/azuredevopskats:2.1.9-build "microbadger.com")

[![Build Status](https://jenkins.navigatorglass.com/buildStatus/icon?job=AzureDevOpsKats/AzureDevOpsKats-base)](https://jenkins.navigatorglass.com/job/AzureDevOpsKats/job/AzureDevOpsKats-base/)
[![Build Status](https://jenkins.navigatorglass.com/buildStatus/icon?job=AzureDevOpsKats/AzureDevOpsKats-api)](https://jenkins.navigatorglass.com/job/AzureDevOpsKats/job/AzureDevOpsKats-api/)

[![Build Status](https://dev.azure.com/AzureDevOpsKats/AzureDevOpsKats/_apis/build/status/stuartshay.AzureDevOpsKats)](https://dev.azure.com/AzureDevOpsKats/AzureDevOpsKats/_build/latest?definitionId=1)

[![Build status](https://ci.appveyor.com/api/projects/status/30ypdshgjhuhmhaw?svg=true)](https://ci.appveyor.com/project/StuartShay/azuredevopskats) [![Build Status](https://travis-ci.org/stuartshay/AzureDevOpsKats.svg?branch=master)](https://travis-ci.org/stuartshay/AzureDevOpsKats)

[![MyGet][data-myget-badge]][data-myget]
[![MyGet][service-myget-badge]][service-myget]


|                          |               |
| ------------------------ | ------------- |
| AzureDevOpsKats.Data     | | [![AzureDevOpsKats.Data package in AzureDevOpsKats feed in Azure Artifacts](https://feeds.dev.azure.com/AzureDevOpsKats/_apis/public/Packaging/Feeds/635e0ad8-8571-488f-82e0-3fb74d47f178/Packages/8dc9496f-5db5-4e61-8894-250d291c94ac/Badge)](https://dev.azure.com/AzureDevOpsKats/AzureDevOpsKats/_packaging?_a=package&feed=635e0ad8-8571-488f-82e0-3fb74d47f178&package=8dc9496f-5db5-4e61-8894-250d291c94ac&preferRelease=true)  |
| AzureDevOpsKats.Service  | |[![AzureDevOpsKats.Service package in AzureDevOpsKats@Local feed in Azure Artifacts](https://feeds.dev.azure.com/AzureDevOpsKats/_apis/public/Packaging/Feeds/635e0ad8-8571-488f-82e0-3fb74d47f178@cb8ef0ed-1b6f-446b-a654-7d71a3c6c5b3/Packages/ba6134fb-0db5-4ffb-a27f-be12b753c8d3/Badge)](https://dev.azure.com/AzureDevOpsKats/AzureDevOpsKats/_packaging?_a=package&feed=635e0ad8-8571-488f-82e0-3fb74d47f178@cb8ef0ed-1b6f-446b-a654-7d71a3c6c5b3&package=ba6134fb-0db5-4ffb-a27f-be12b753c8d3&preferRelease=true) |


[data-myget]: https://www.myget.org/feed/azuredevopskats/package/nuget/AzureDevOpsKats.Data
[data-myget-badge]: https://img.shields.io/myget/azuredevopskats/v/AzureDevOpsKats.Data.svg?label=AzureDevOpsKats.Data
[service-myget]: https://www.myget.org/feed/azuredevopskats/package/nuget/AzureDevOpsKats.Service
[service-myget-badge]: https://img.shields.io/myget/azuredevopskats/v/AzureDevOpsKats.Service.svg?label=AzureDevOpsKats.Service

## Instalation & Run Instructions

```
git clone https://github.com/stuartshay/AzureDevOpsKats.git

cd AzureDevOpsKats
dotnet restore

cd src\AzureDevOpsKats.Web\
dotnet run
```

### Cake

Windows 

```
set-executionpolicy unrestricted

.\build.ps1
```

Linux/Mac
```
chmod +x build.sh

./build.sh
```

## Web Site

The Site can be accesed at the following url

```
http://localhost:5000/
```

![](assets/web.png)

## Swagger API Documentation

```
http://localhost:5000/swagger/index.html
```
![](assets/swagger.png)
