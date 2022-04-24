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
module "security_group_alb" {
  source = "../modules/security-group//modules/http"

  name   = "${local.realm_name}-for-alb"
  vpc_id = data.terraform_remote_state.network.outputs.vpc_id

  cidr_ingresses = [
    "0.0.0.0/0"
  ]
}

module "security_group_ecs_tasks" {
  source = "../modules/security-group"

  name   = "${local.realm_name}-for-api"
  vpc_id = data.terraform_remote_state.network.outputs.vpc_id

  sg_ingresses = {
    "5000" = {
      from_port         = 5000
      to_port           = 5000
      protocol          = "tcp"
      security_group_id = module.security_group_alb.id
    },
    "80" = {
      from_port         = 80
      to_port           = 80
      protocol          = "tcp"
      security_group_id = module.security_group_alb.id
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

# ALB
module "alb" {
  source = "../modules/alb"

  name               = local.realm_name
  vpc_id             = data.terraform_remote_state.network.outputs.vpc_id
  subnet_ids         = data.terraform_remote_state.network.outputs.public_subnet_ids
  security_group_ids = [module.security_group_alb.id]
  enable_https       = false
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
