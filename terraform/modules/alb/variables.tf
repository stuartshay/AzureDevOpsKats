variable "name" {
  description = "The ALB name"
  type        = string
}

variable "vpc_id" {
  description = "VPC ID for instance"
  type        = string
}

variable "subnet_ids" {
  description = "VPC subnets for ALB"
  type        = list(string)
  default     = []
}

variable "security_group_ids" {
  description = "ALB security groups"
  type        = list(string)
  default     = []
}

variable "health_check_path" {
  description = "Backend service health check path"
  type        = string
  default     = "/health"
}

variable "enable_https" {
  description = "Should or should not enable HTTPS"
  type        = bool
  default     = false
}

variable "acm_certificate_arn" {
  description = "The ACM certficate ARN"
  type        = string
  default     = ""
}
