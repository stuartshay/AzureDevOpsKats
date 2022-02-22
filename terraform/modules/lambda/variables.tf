variable "name" {
  description = "The Lambda function name"
  type        = string
}

variable "ecs_cluster_name" {
  description = "The ECS cluster name"
  type        = string
}

variable "runtime" {
  description = "The Lambda function runtime"
  type        = string
  default     = "python3.8"
}

variable "memory" {
  description = "The Lambda function runtime"
  type        = number
  default     = 10240
}

variable "timeout" {
  description = "The Lambda function runtime"
  type        = number
  default     = 900
}

variable "tags" {
  description = "Add more tags"
  type        = map(string)
  default     = {}
}
