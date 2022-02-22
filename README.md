# AzureDevOpsKats

[![CI/CD Build/Test/Deploy](https://github.com/stuartshay/AzureDevOpsKats/actions/workflows/ci-cd-action.yml/badge.svg)](https://github.com/stuartshay/AzureDevOpsKats/actions/workflows/ci-cd-action.yml)

[![This image on DockerHub](https://img.shields.io/docker/pulls/stuartshay/azuredevopskats.svg)](https://hub.docker.com/r/stuartshay/azuredevopskats/)

```
master-devops-1727857016.us-east-1.elb.amazonaws.com
```

## Purpose

The Kats Club has an extensive collection of Kats Photos and is looking for a development platform where they can manage their large archive of photos.

The Kats Club is a Agile Development and DevOps team and has embraced the following technologies for there next generation platform C#, CI/CD, Containerization and the Cloud

### AWSDevOpsKats

The Team has chosen the AWS Elastic Container Service (ECS) to host the club’s infrastructure and is utilizing the following technologies in their development and release pipeline.

- [Terraform Infrastructure](/terraform)
- [ECS Service Tasks Deployment](/ecspresso)
- [GitHub Actions Workflows](https://github.com/stuartshay/AzureDevOpsKats/actions)

### Development

- [C# Coding Standards](/docfx/articles/csharp_coding_standards.md)
- [Developer DocFx Documentation](https://stuartshay.github.io/AzureDevOpsKats/)

![](assets/web.png)

## Builds

| Job                           | Agent        | Status                                                                                                                                                                                                     |
| ----------------------------- | ------------ | ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| Windows/Linux Automated Build | Appveyor     | [![Build status](https://ci.appveyor.com/api/projects/status/30ypdshgjhuhmhaw?svg=true)](https://ci.appveyor.com/project/StuartShay/azuredevopskats)                                                       |
| Windows/Linux Automated Build | Azure Devops | [![Build Status](https://dev.azure.com/AzureDevOpsKats/AzureDevOpsKats/_apis/build/status/stuartshay.AzureDevOpsKats)](https://dev.azure.com/AzureDevOpsKats/AzureDevOpsKats/_build/latest?definitionId=1) |
| Docker x86/Arm7 Image         | Jenkins      | [![Build Status](https://jenkins.navigatorglass.com/buildStatus/icon?job=AzureDevOpsKats/AzureDevOpsKats-multi)](https://jenkins.navigatorglass.com/job/AzureDevOpsKats/job/AzureDevOpsKats-multi/)        |

| Library                 | Nuget Repository                               | MyGet Repository                               |
| ----------------------- | ---------------------------------------------- | ---------------------------------------------- |
| AzureDevOpsKats.Data    | [![Nuget][data-nuget-badge]][data-nuget]       | [![MyGet][data-myget-badge]][data-myget]       |
| AzureDevOpsKats.Service | [![Nuget][service-nuget-badge]][service-nuget] | [![MyGet][service-myget-badge]][service-myget] |

[data-myget]: https://www.myget.org/feed/azuredevopskats/package/nuget/AzureDevOpsKats.Data
[data-myget-badge]: https://img.shields.io/myget/azuredevopskats/v/AzureDevOpsKats.Data.svg?label=AzureDevOpsKats.Data
[data-nuget]: https://dev.azure.com/AzureDevOpsKats/AzureDevOpsKats/_packaging?_a=package&feed=635e0ad8-8571-488f-82e0-3fb74d47f178@cb8ef0ed-1b6f-446b-a654-7d71a3c6c5b3&package=ba6134fb-0db5-4ffb-a27f-be12b753c8d3&preferRelease=true
[data-nuget-badge]: https://feeds.dev.azure.com/AzureDevOpsKats/_apis/public/Packaging/Feeds/635e0ad8-8571-488f-82e0-3fb74d47f178@cb8ef0ed-1b6f-446b-a654-7d71a3c6c5b3/Packages/ba6134fb-0db5-4ffb-a27f-be12b753c8d3/Badge
[service-myget]: https://www.myget.org/feed/azuredevopskats/package/nuget/AzureDevOpsKats.Service
[service-myget-badge]: https://img.shields.io/myget/azuredevopskats/v/AzureDevOpsKats.Service.svg?label=AzureDevOpsKats.Service
[service-nuget]: https://dev.azure.com/AzureDevOpsKats/AzureDevOpsKats/_packaging?_a=package&feed=635e0ad8-8571-488f-82e0-3fb74d47f178&package=ba6134fb-0db5-4ffb-a27f-be12b753c8d3&preferRelease=true
[service-nuget-badge]: https://feeds.dev.azure.com/AzureDevOpsKats/_apis/public/Packaging/Feeds/635e0ad8-8571-488f-82e0-3fb74d47f178/Packages/ba6134fb-0db5-4ffb-a27f-be12b753c8d3/Badge

## Instalation & Run Instructions

```
git clone https://github.com/stuartshay/AzureDevOpsKats.git

cd AzureDevOpsKats
dotnet restore

cd src\AzureDevOpsKats.Web\
dotnet run
```

```
http://localhost:5000/
```

### Build Commands

| Build Type        | Linux/Mac                    | Windows                       |
| ----------------- | ---------------------------- | ----------------------------- |
| CI Build          | ./build.sh --target=CI-Build | .\build.ps1 --target=CI-Build |
| SonarQube Testing | ./build.sh --target=sonar    | .\build.ps1 --target=sonar    |

### Documentation

```
https://stuartshay.github.io/AzureDevOpsKats/
```
