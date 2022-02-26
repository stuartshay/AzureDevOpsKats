variable "name" {
  description = "The security group name"
  type        = string
}

variable "vpc_id" {
  description = "The VPC ID"
  type        = string
}

variable "cidr_ingresses" {
  description = "The security group ingress rules source from CIDR"
  type        = list(string)
  default     = []
}

variable "sg_ingresses" {
  description = "The security group ingress rules source from security groups"
  type        = list(string)
  default     = []
}
