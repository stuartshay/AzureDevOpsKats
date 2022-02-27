output "ecs_task_execution_role_arn" {
  description = "The ECS execution role arn"
  value       = aws_iam_role.ecs_task_execution.arn
}

output "ecs_container_role_arn" {
  description = "The ECS container role arn"
  value       = aws_iam_role.ecs_container.arn
}
