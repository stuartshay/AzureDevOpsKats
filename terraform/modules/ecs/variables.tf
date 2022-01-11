variable "name" {
  description = "The ECS cluster name"
  type        = string
}

variable "execution_role_arn" {
  description = "The ECS execution role ARN"
  type        = string
}

variable "task_role_arn" {
  description = "The ECS task role ARN"
  type        = string
}
