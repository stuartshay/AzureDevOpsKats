# Data source
data "terraform_remote_state" "shared" {
  backend = "s3"

  config = {
    bucket  = "devops-team-tfstate"
    key     = "devops/aws/us-east-1/s3/devopskats/shared"
    region  = "${local.region}"
    profile = "awsdevopskats"
  }
}

data "terraform_remote_state" "network" {
  backend = "s3"

  config = {
    bucket  = "devops-team-tfstate"
    key     = "devops/aws/us-east-1/s3/devopskats/common/network/${local.env}"
    region  = "${local.region}"
    profile = "awsdevopskats"
  }
}

data "terraform_remote_state" "storage" {
  backend = "s3"

  config = {
    bucket  = "devops-team-tfstate"
    key     = "devops/aws/us-east-1/s3/devopskats/storage/${local.env}"
    region  = "${local.region}"
    profile = "awsdevopskats"
  }
}

# Security groups
module "security_group_ecs_tasks" {
  source = "../modules/security-group"

  name   = "${local.realm_name}-for-api"
  vpc_id = data.terraform_remote_state.network.outputs.vpc_id

  cidr_ingresses = {
    "80" = {
      from_port   = 80
      to_port     = 80
      protocol    = "tcp"
      cidr_blocks = ["0.0.0.0/0"]
    },
    "443" = {
      from_port   = 443
      to_port     = 443
      protocol    = "tcp"
      cidr_blocks = ["0.0.0.0/0"]
    }
  }
}

resource "aws_security_group_rule" "efs" {
  type                     = "ingress"
  from_port                = 2049
  to_port                  = 2049
  protocol                 = "tcp"
  security_group_id        = data.terraform_remote_state.storage.outputs.efs_sg_id
  source_security_group_id = module.security_group_ecs_tasks.id
}

# ECS
module "ecs" {
  source = "../modules/ecs"

  name           = local.realm_name
  log_group_name = local.realm_name

  execution_role_arn = data.terraform_remote_state.shared.outputs.ecs_task_execution_role_arn
  task_role_arn      = data.terraform_remote_state.shared.outputs.ecs_container_role_arn
}

# SSM Params
module "ssm" {
  source = "../modules/ssm"

  name = local.realm_name
}

# Lambda function
module "lambda" {
  source = "../modules/lambda"

  name             = local.realm_name
  ecs_cluster_name = module.ecs.cluster_name
}
