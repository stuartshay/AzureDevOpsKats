variable "name" {
  description = "The EFS name"
  type        = string
}

variable "subnet_id" {
  description = "A VPC subnet for EFS"
  type        = list(string)
}

variable "security_group_ids" {
  description = "EFS security groups"
  type        = list(string)
}

variable "tags" {
  description = "Add more tags"
  type        = map(string)
  default     = {}
}
