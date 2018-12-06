output "kube_config" {
  value = "${azurerm_kubernetes_cluster.mod.kube_config_raw}"
}

output "admin_host" {
  value = "${azurerm_kubernetes_cluster.mod.kube_config.0.host}"
}

output "ingress_host" {
  value = "${dnsimple_record.test.hostname}"
}
