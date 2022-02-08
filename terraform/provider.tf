terraform {
  required_version = "> 0.15.0"
  required_providers {
    aws = {
      source  = "hashicorp/aws"
      version = "~> 3.67"
    }
  }

  # backend "s3" {
  #   bucket  = "devops-team-tfstate"
  #   encrypt = true
  #   key     = "devops/aws/us-east-1/s3/devopskats"
  #   region  = "us-east-1"
  #   profile = "stuartshay"
  # }

  cloud {
    organization = "DevOpsKats"

    workspaces {
      name = "AWSDevOpsKats"
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
