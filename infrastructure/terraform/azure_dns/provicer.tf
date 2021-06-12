provider "azurerm" {
  version = "2.63"
}

provider "dnsimple" {
  version = "0.1.0"

  token   = "${var.dnsimple_token}"
  account = "${var.dnsimple_account}"
}
