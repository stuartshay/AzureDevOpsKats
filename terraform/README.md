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
├── environments
│   ├── develop
│   │   └── main.tf
│   └── master
│       └── main.tf
|
├── modules
│   ├── ecs
│   │   └── main.tf
│   ├── iam
│   │   └── main.tf
│   ├── jumpbox
│   │   └── main.tf
│   ├── lambda
│   │   └── main.tf
│   ├── management
│   │   └── main.tf
│   ├── networking
│   │   └── main.tf
│   ├── storage
│   │   └── main.tf
|
├── templates
├── main.tf
├── variables.tf

```
