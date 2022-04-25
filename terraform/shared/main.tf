# IAM
module "iam" {
  source = "../modules/iam"

  name = local.name
}
