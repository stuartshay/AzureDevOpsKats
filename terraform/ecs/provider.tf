terraform {
  required_version = "> 0.15"
  required_providers {
    aws = {
      source  = "hashicorp/aws"
      version = "~> 3.67"
    }
  }

  cloud {
    organization = "DevOpsKats"

    workspaces {
      name = "AWSDevOpsKats-ECS"
    }
  }
}

provider "aws" {
  region  = "us-east-1"
  profile = "stuartshay"

  default_tags {
    tags = {
      application = "devopskats"
      env         = "development"
      owner       = "culiops"
    }
  }
}