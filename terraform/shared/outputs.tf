output "ecs_task_execution_role_arn" {
  description = "The ECS execution role arn"
  value       = module.iam.ecs_task_execution_role_arn
}

output "ecs_container_role_arn" {
  description = "The ECS container role arn"
  value       = module.iam.ecs_container_role_arn
}
