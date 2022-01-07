variable "name" {
  description = "The ECS cluster name"
  type        = string
}

# variable "subnet_ids" {
#   description = "VPC subnets for instances"
#   type        = list(string)
#   default     = []
# }

# variable "security_group_ids" {
#   description = "Instance security groups"
#   type        = list(string)
#   default     = []
# }

variable "execution_role_arn" {
  description = "The ECS execution role ARN"
  type        = string
}

variable "task_role_arn" {
  description = "The ECS task role ARN"
  type        = string
}
