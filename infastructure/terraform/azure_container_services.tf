resource "azurerm_kubernetes_cluster" "mod" {
  name                = "${var.env_name}"
  location            = "${azurerm_resource_group.mod.location}"
  resource_group_name = "${azurerm_resource_group.mod.name}"
  dns_prefix          = "${var.prject}"

  linux_profile {
    admin_username = "Navigator"

    ssh_key {
      key_data = "${var.ssh_public_key}"
    }
  }

  agent_pool_profile {
    name            = "default"
    count           = "1"
    vm_size         = "Standard_A2_v2"
    os_type         = "Linux"
    os_disk_size_gb = 30
  }

  service_principal {
    client_id     = "${var.service_principal_client_id}"
    client_secret = "${var.service_principal_client_secret}"
  }

  tags {
    Environment = "${var.env_name}"
  }
}
