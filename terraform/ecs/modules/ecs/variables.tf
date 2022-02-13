variable "log_group_name" {
  description = "The log group name"
  type        = string
}

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

variable "tags" {
  description = "ALB more tags"
  type        = map(string)
  default     = {}
}
