# AWSDevOpsKats Terraform

AWS ECS Infrastructure Deployment using Terraform, Ansibile and GitHub Actions

[![Deploy Core Infrastructures](https://github.com/stuartshay/AzureDevOpsKats/actions/workflows/deploy-core-infra.yml/badge.svg)](https://github.com/stuartshay/AzureDevOpsKats/actions/workflows/deploy-core-infra.yml)

[![Deploy Shared Infrastructure](https://github.com/stuartshay/AzureDevOpsKats/actions/workflows/deploy-shared-infra.yml/badge.svg)](https://github.com/stuartshay/AzureDevOpsKats/actions/workflows/deploy-shared-infra.yml)

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