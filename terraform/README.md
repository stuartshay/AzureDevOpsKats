# AWSDevOpsKats Terraform

AWS ECS Infrastructure Deployment using Terraform Cloud, Ansibile and GitHub Actions

### Terraform Cloud Workspaces

```
https://app.terraform.io/app/DevOpsKats/workspaces
```

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
│       └── main.tf


|
├── templates
├── main.tf
├── variables.tf

```
