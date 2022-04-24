# Data source
data "terraform_remote_state" "network" {
  backend = "s3"

  config = {
    bucket  = "devops-team-tfstate"
    key     = "devops/aws/us-east-1/s3/devopskats/common/network/${local.env}"
    region  = "${local.region}"
    profile = "awsdevopskats"
  }
}

data "terraform_remote_state" "jumpbox" {
  backend = "s3"

  config = {
    bucket  = "devops-team-tfstate"
    key     = "devops/aws/us-east-1/s3/devopskats/common/jumpbox/${local.env}"
    region  = "${local.region}"
    profile = "awsdevopskats"
  }
}

# Security groups
module "security_group_efs" {
  source = "../../modules/security-group"

  name   = "${local.realm_name}-for-efs"
  vpc_id = data.terraform_remote_state.network.outputs.vpc_id

  sg_ingresses = {
    # "ecs_tasks" = {
    #   from_port         = 2049
    #   to_port           = 2049
    #   protocol          = "tcp"
    #   security_group_id = module.security_group_ecs_tasks.id
    # },
    "jumpbox" = {
      from_port         = 2049
      to_port           = 2049
      protocol          = "tcp"
      security_group_id = data.terraform_remote_state.jumpbox.outputs.security_group_id
    }
  }
}

# EFS
module "efs" {
  source = "../../modules/efs"

  name               = local.realm_name
  subnet_ids         = data.terraform_remote_state.network.outputs.public_subnet_ids
  security_group_ids = [module.security_group_efs.id]
}
