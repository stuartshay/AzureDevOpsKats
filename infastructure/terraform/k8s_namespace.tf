resource "kubernetes_namespace" "mod" {
  metadata {
    name = "${var.subdomain}"
  }
}
