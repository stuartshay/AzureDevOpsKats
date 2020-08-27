provider "azurerm" {
  version = "1.44"
}

provider "dnsimple" {
  version = "0.1.0"

  token   = "${var.dnsimple_token}"
  account = "${var.dnsimple_account}"
}
