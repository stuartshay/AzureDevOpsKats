# Data source
data "terraform_remote_state" "shared" {
  backend = "s3"

  config = {
    bucket = "devops-team-tfstate"
    key    = "devops/aws/us-east-1/s3/devopskats/shared"
    region = "${local.region}"
    profile = "stuartshay"
  }
}

# Security groups
module "security_group_alb" {
  source = "../modules/security-group//modules/http"

  name   = "${local.realm_name}-for-alb"
  vpc_id = local.vpc_id

  cidr_ingresses = [
    "0.0.0.0/0"
  ]
}

module "security_group_ecs_tasks" {
  source = "../modules/security-group"

  name   = "${local.realm_name}-for-api"
  vpc_id = local.vpc_id

  sg_ingresses = {
    "ecs_tasks" = {
      from_port         = 5000
      to_port           = 5000
      protocol          = "tcp"
      security_group_id = module.security_group_alb.id
    }
  }
}

module "security_group_efs" {
  source = "../modules/security-group"

  name   = "${local.realm_name}-for-efs"
  vpc_id = local.vpc_id

  sg_ingresses = {
    "ecs_tasks" = {
      from_port         = 2049
      to_port           = 2049
      protocol          = "tcp"
      security_group_id = module.security_group_ecs_tasks.id
    },
    "jumpbox" = {
      from_port         = 2049
      to_port           = 2049
      protocol          = "tcp"
      security_group_id = data.terraform_remote_state.shared.outputs.jumpbox_sg_id
    }
  }
}

# ALB
module "alb" {
  source = "../modules/alb"

  name               = local.realm_name
  vpc_id             = local.vpc_id
  subnet_ids         = local.subnet_ids
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

# EFS
module "efs" {
  source = "../modules/efs"

  name               = local.realm_name
  subnet_id          = local.subnet_ids[0]
  security_group_ids = [module.security_group_efs.id]
}

# Lambda function
module "lambda" {
  source = "../modules/lambda"

  name               = local.realm_name
  ecs_cluster_name = module.ecs.cluster_name
}