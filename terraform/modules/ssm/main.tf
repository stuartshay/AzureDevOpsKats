resource "aws_ssm_parameter" "smtp_password" {
  name  = "/${var.name}/smtp/password"
  type  = "SecureString"
  value = "test"
}

resource "aws_ssm_parameter" "smtp_port" {
  name  = "/${var.name}/smtp/port"
  type  = "String"
  value = "25"
}

resource "aws_ssm_parameter" "smtp_server" {
  name  = "/${var.name}/smtp/server"
  type  = "String"
  value = "localhost"
}

resource "aws_ssm_parameter" "smtp_username" {
  name  = "/${var.name}/smtp/username"
  type  = "String"
  value = "test"
}
