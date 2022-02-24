# IAM
module "iam" {
  source = "../modules/iam"

  name = local.name
}

# Jumpbox
module "jumpbox" {
  source = "../modules/jumpbox"

  name = "${local.name}-jumpbox"
  vpc_id = local.vpc_id
  subnet_id = local.subnet_ids[0]
}