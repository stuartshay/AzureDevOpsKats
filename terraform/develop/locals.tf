locals {
  name       = "devopskats"
  env        = "develop"
  region     = "us-east-1"
  realm_name = "${local.name}-${local.env}"

  tags = {
    application = local.name
    env         = local.env
    owner       = "devops"
  }

  vpc_id     = data.aws_vpc.default.id
  subnet_ids = data.aws_subnets.public.ids
}
