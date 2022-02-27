terraform {
  required_version = "> 0.15"
  required_providers {
    aws = {
      source  = "hashicorp/aws"
      version = "~> 3.67"
    }
  }
  backend "s3" {
    bucket  = "devops-team-tfstate"
    encrypt = true
    key     = "devops/aws/us-east-1/s3/devopskats/master"
    region  = "us-east-1"
    profile = "awsdevopskats"
  }
}

provider "aws" {
  region  = "us-east-1"
  profile = "awsdevopskats"

  default_tags {
    tags = local.tags
  }
}