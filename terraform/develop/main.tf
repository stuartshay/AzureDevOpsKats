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

# Security groups
module "security_group_ecs_tasks" {
  source = "../modules/security-group"

  name   = "${local.realm_name}-for-api"
  vpc_id = local.vpc_id

  cidr_ingresses = {
    "all" = {
      from_port   = 5000
      to_port     = 5000
      protocol    = "tcp"
      cidr_blocks = ["0.0.0.0/0"]
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

# SSM Params
module "ssm" {
  source = "../modules/ssm"

  name = local.realm_name
}