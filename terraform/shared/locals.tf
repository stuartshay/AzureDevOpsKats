locals {
  name        = "devopskats"
  env         = "shared"
  region      = "ap-southeast-1"
  realm_name = "${local.name}-${local.env}"

  tags = {
    application   = local.name
    env   = local.environment
    owner         = "devops"
  }
}
