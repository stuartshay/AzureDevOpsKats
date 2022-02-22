# ECS Service Tasks

[![Deploy ECS Service Tasks](https://github.com/stuartshay/AzureDevOpsKats/actions/workflows/deploy-ecs-service-tasks.yml/badge.svg)](https://github.com/stuartshay/AzureDevOpsKats/actions/workflows/deploy-ecs-service-tasks.yml)

![](../assets/ecs-service-workflow.png)

## test-coverage

### build

- .NET Build
- Create Docker Image
- Push Image to Amazon ECR

## deploy

Depoly ECS task definitions

ecspresso is a deployment tool for Amazon ECS.

```
https://github.com/kayac/ecspresso
```

### develop branch

- Git Event : Push

### master branch

- Git Event : Push, Pull Request

## health-check

- Check ECS Deployment and Application & Health

[Health Check Endpoint](http://master-devops-1727857016.us-east-1.elb.amazonaws.com/health)

## selenium-test

- Application Smoke and UI Testing

| Test Type                                  | Docker Image                                                                                                                                                                 |
| ------------------------------------------ | ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| [selenium-screenshot](../docker/selenium/) | [![This image on DockerHub](https://img.shields.io/docker/pulls/stuartshay/azuredevopskats-selenium.svg)](https://hub.docker.com/r/stuartshay/azuredevopskats-selenium/tags) |
