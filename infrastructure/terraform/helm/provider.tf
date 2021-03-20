# Initialize Helm (and install Tiller)
provider "helm" {
  version = "0.10.6"

  install_tiller = true
  home           = "${path.root}/../.helm"
}
