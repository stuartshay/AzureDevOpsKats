resource "random_id" "azurerm_resource_group_mod" {
  byte_length = 4
}

resource "azurerm_resource_group" "mod" {
  name     = "${var.prject}${var.pipeline}${format("%s", random_id.azurerm_resource_group_mod.hex)}"
  location = "${var.region}"

  tags {
    environment = "${var.env_name}"
  }
}
