# AWSDevOpsKats Terraform

AWS ECS Infrastructure Deployment using Terraform, Ansibile and GitHub Actions

AWS Fargate is a compute engine for Amazon ECS that allows you to run containers
without having to manage servers or clusters. With AWS Fargate, you no longer have to
provision, configure, and scale clusters of virtual machines to run containers. This removes
the need to choose server types, decide when to scale your clusters or optimize cluster
packing. AWS Fargate removes the need for you to interact with or think about servers or
clusters. Fargate lets you focus on designing and building your applications instead of
managing the infrastructure that runs them.

## Infrastructure

### VPC

[![Deploy VPC resources](https://github.com/stuartshay/WorkflowCommon/actions/workflows/deploy-vpc.yml/badge.svg)](https://github.com/stuartshay/WorkflowCommon/actions/workflows/deploy-vpc.yml)

### Storage

[![Infrastructure - Deploy Storage](https://github.com/stuartshay/AzureDevOpsKats/actions/workflows/deploy-storage.yml/badge.svg)](https://github.com/stuartshay/AzureDevOpsKats/actions/workflows/deploy-storage.yml)

- EFS Volumes

| Name                      | Volume Size |
| ------------------------- | ----------- |
| devopskats-master-images  | 1 Gib       |
| devopskats-develop-images | 1 Gib       |

### Security

[![Infrastructure - Deploy Security](https://github.com/stuartshay/AzureDevOpsKats/actions/workflows/deploy-security-infra.yml/badge.svg)](https://github.com/stuartshay/AzureDevOpsKats/actions/workflows/deploy-security-infra.yml)

- IAM Roles and policies for ECS Tasks
- Parameter Store Variables and Secrets

### Configuration

- TODO Move Parameter Store Variables and Secrets

### ALB

- TODO MOVE TO ALB SINGLE DEPLOYMENT

[![Infrastructure - Deploy ALB](https://github.com/stuartshay/AzureDevOpsKats/actions/workflows/deploy-alb.yml/badge.svg)](https://github.com/stuartshay/AzureDevOpsKats/actions/workflows/deploy-alb.yml)

### Infrastructure

- ECS Clusters
- Application Load Balancer (Public facing)

### Project Structure

```

├── ansible
│
├── develop
|    └── main.tf
|
│── master
│    └── main.tf
|
├── modules
│   ├── alb
│   │   └── main.tf
│   ├── ecs
│   │   └── main.tf
│   ├── efs
│   │   └── main.tf
│   ├── iam
│   │   └── main.tf
│   ├── jumpbox
│   │   └── main.tf
│   ├── lambda
│   │   └── main.tf
│   ├── security-group
│   │   └── main.tf
│   └── ssm
|       └── main.tf
├── shared

```
