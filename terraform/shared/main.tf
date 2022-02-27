# IAM
module "iam" {
  source = "../modules/iam"

  name = local.name
}

# Jumpbox
module "jumpbox" {
  source = "../modules/jumpbox"

<<<<<<< HEAD
  name      = "${local.name}-jumpbox"
  vpc_id    = local.vpc_id
=======
  name = "${local.name}-jumpbox"
  vpc_id = local.vpc_id
>>>>>>> origin/master
  subnet_id = local.subnet_ids[0]
}