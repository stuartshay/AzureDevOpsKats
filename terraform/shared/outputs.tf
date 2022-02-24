output "ecs_task_execution_role_arn" {
  description = "The ECS execution role arn"
  value       = module.iam.ecs_task_execution_role_arn
}

output "ecs_container_role_arn" {
  description = "The ECS container role arn"
  value       = module.iam.ecs_container_role_arn
}

output "jumpbox_sg_id" {
  description = "The jumpbox security group ID"
  value       = module.jumpbox.security_group_id
}