# Initialize Helm (and install Tiller)
provider "helm" {
  version = "0.6.2"

  install_tiller = true
  home           = "${path.root}/../.helm"
}
