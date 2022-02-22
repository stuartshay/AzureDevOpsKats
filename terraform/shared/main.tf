module "iam" {
  source = "../modules/iam"

  name           = "${local.realm_name}"
}