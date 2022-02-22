locals {
  name        = "devopskats"
  env         = "production"
  region      = "ap-southeast-1"
  realm_name = "${local.name}-${local.env}"

  tags = {
    application   = local.name
    env   = local.environment
    owner         = "devops"
  }

  vpc_id             = ""
  public_subnet_ids  = []
  private_subnet_ids = []

  sg_cidr_blocks = []

  other_sg_ids = [
  ]
}
