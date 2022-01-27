resource "aws_ssm_parameter" "master_smtp_password" {
  name  = "/${local.project_name}/master/smtp/password"
  type  = "SecureString"
  value = "test"

  tags = {
    Env = "master"
  }
}

resource "aws_ssm_parameter" "master_smtp_port" {
  name  = "/${local.project_name}/master/smtp/port"
  type  = "String"
  value = "25"

  tags = {
    Env = "master"
  }
}

resource "aws_ssm_parameter" "master_smtp_server" {
  name  = "/${local.project_name}/master/smtp/server"
  type  = "String"
  value = "localhost"

  tags = {
    Env = "master"
  }
}

resource "aws_ssm_parameter" "master_smtp_username" {
  name  = "/${local.project_name}/master/smtp/username"
  type  = "String"
  value = "test"

  tags = {
    Env = "master"
  }
}


resource "aws_ssm_parameter" "develop_smtp_password" {
  name  = "/${local.project_name}/develop/smtp/password"
  type  = "SecureString"
  value = "test"

  tags = {
    Env = "develop"
  }
}

resource "aws_ssm_parameter" "develop_smtp_port" {
  name  = "/${local.project_name}/develop/smtp/port"
  type  = "String"
  value = "25"

  tags = {
    Env = "develop"
  }
}

resource "aws_ssm_parameter" "develop_smtp_server" {
  name  = "/${local.project_name}/develop/smtp/server"
  type  = "String"
  value = "localhost"

  tags = {
    Env = "develop"
  }
}

resource "aws_ssm_parameter" "develop_smtp_username" {
  name  = "/${local.project_name}/develop/smtp/username"
  type  = "String"
  value = "test"

  tags = {
    Env = "develop"
  }
}
