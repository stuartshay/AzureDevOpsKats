# Initialize Helm (and install Tiller)
provider "helm" {
  version = "2.2.0"

  install_tiller = true
  home           = "${path.root}/../.helm"
}
