locals {
  name       = "devopskats"
  env        = "master"
  region     = "us-east-1"
  realm_name = "${local.name}-${local.env}"

  tags = {
    application = local.name
    env         = local.env
    owner       = "devops"
  }
}
