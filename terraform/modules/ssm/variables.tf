variable "name" {
  description = "The project name"
  type        = string
}

variable "tags" {
  description = "Add more tags"
  type        = map(string)
  default     = {}
}
