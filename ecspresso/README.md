# ECS Service Tasks

[![Deploy ECS Service Tasks](https://github.com/stuartshay/AzureDevOpsKats/actions/workflows/deploy-ecs-service-tasks.yml/badge.svg)](https://github.com/stuartshay/AzureDevOpsKats/actions/workflows/deploy-ecs-service-tasks.yml)

ECS Service Tasks Workflow Deployment

![](../assets/ecs-service-workflow.png)

## Workflow Dispatch

Overide Workflow Defaults

![](../assets/ecs-service-workflow-dispatch.png)

ECS Cluster Deployment

| Branch               | ECS Cluster        |
| -------------------- | ------------------ |
| master               | devopskats-master  |
| devlop, feature, fix | devopskats-develop |

Master Branch Deployment override

| ECS Desired Count |                             |
| ----------------- | --------------------------- |
| 1,2,5             | Number of Fargate ECS Tasks |
| 0                 | Terminate all Tasks         |

## Workflow Steps

### test-coverage

[![CI/CD Build/Test/Deploy](https://github.com/stuartshay/AzureDevOpsKats/actions/workflows/ci-cd-action.yml/badge.svg)](https://github.com/stuartshay/AzureDevOpsKats/actions/workflows/ci-cd-action.yml)

- .NET Build & Lint
- .NET Unit Testing and Code Coverage
- Image Vulnerability Scanning

### build

- .NET Build
- Create Docker Image
- [Push Image to Amazon ECR](https://aws.amazon.com/ecr/)

| Repository      | Tag Format                                                   | Example                     |
| --------------- | ------------------------------------------------------------ | --------------------------- |
| azuredevopskats | { net_core_ver }.{ github.run_number }-buildx-{ github.sha } | 6.0.17-buildx-232b92de90ff3 |

### deploy

Deploy ECS Fargate Container and task definition

- [Set Fargate service/task definition parameters](https://docs.aws.amazon.com/AmazonECS/latest/developerguide/task_definition_parameters.html)

- [Set Container Secrets & Environment Variables](https://aws.amazon.com/systems-manager/)

| Variable Type | Name                             |
| ------------- | -------------------------------- |
| Environment   | ASPNETCORE_ENVIRONMENT           |
| Environment   | CLUSTER_NAME                     |
| Secret        | /devopskats/{ENVIRONMENT}/secret |

- [Set Environment Loggers](https://aws.amazon.com/cloudwatch/)

| Log Type         | Log Group                |
| ---------------- | ------------------------ |
| Container Logger | devopskats-{ENVIRONMENT} |

- [Mount Common EFS Storage Volumes](https://aws.amazon.com/efs/)

| Container Path | Storage Type | Volume Name       |
| -------------- | ------------ | ----------------- |
| /images        | EFS          | efs-{ENVIRONMENT} |

#### Modify ====> efs-devopskats-{ENVIRONMENT}-images

- [Terraform State]()

| Build Assets | Storage Type | Bucket                              |
| ------------ | ------------ | ----------------------------------- |
| tfstate      | S3 Bucket    | /devops/aws/us-east-1/s3/devopskats |

#### develop branch

- Git Event : Push

#### master branch

- Git Event : Push, Pull Request

#### Ecspresso Deployment Tool

```
https://github.com/kayac/ecspresso
```

### health-check

- Check ECS Deployment and Application Health
- Validate Release Tag Matches Running Container Tag

[Health Check Endpoint](http://master-devops-1727857016.us-east-1.elb.amazonaws.com/health)

### selenium-test

- Application Smoke and UI Testing

| Test Type                                  | Workflow                                                                                                                                                                                                       | Docker Image                                                                                                                                                                 |
| ------------------------------------------ | -------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- | ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| [selenium-screenshot](../docker/selenium/) | [![Selenium Workflow](https://github.com/stuartshay/AzureDevOpsKats/actions/workflows/selenium.workflow.yml/badge.svg)](https://github.com/stuartshay/AzureDevOpsKats/actions/workflows/selenium.workflow.yml) | [![This image on DockerHub](https://img.shields.io/docker/pulls/stuartshay/azuredevopskats-selenium.svg)](https://hub.docker.com/r/stuartshay/azuredevopskats-selenium/tags) |
