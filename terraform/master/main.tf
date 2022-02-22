data "terraform_remote_state" "shared" {
  backend = "remote"

  config = {
    organization = "DevOpsKats"
    workspaces = {
      name = "AWSDevOpsKats-Shared"
    }
  }
}


# ECS
module "ecs" {
  count = length(local.envs)

  source = "../modules/ecs"

  name           = "${local.realm_name}"
  log_group_name = local.realm_name

  execution_role_arn = data.terraform_remote_state.shared.outputs.ecs_task_execution_role_arn
  task_role_arn      = data.terraform_remote_state.shared.outputs.ecs_container_role_arn
}