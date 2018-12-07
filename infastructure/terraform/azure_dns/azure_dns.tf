resource "azurerm_resource_group" "mod" {
  name     = "AzureDNS"
  location = "${var.region}"
}

resource "azurerm_dns_zone" "test" {
  name                = "${var.domain_name}"
  resource_group_name = "${azurerm_resource_group.mod.name}"
  zone_type           = "Public"
}

output "azure_name_servers" {
  value = "${azurerm_dns_zone.test.name_servers}"
}
