provider "azurerm" {
  version = "1.19"
}

provider "dnsimple" {
  version = "0.5.3"

  token   = "${var.dnsimple_token}"
  account = "${var.dnsimple_account}"
}
