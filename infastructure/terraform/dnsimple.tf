resource "dnsimple_record" "test" {
  domain = "${var.domain_name}"
  name   = "${var.subdomain}"
  value  = "${azurerm_public_ip.nginx_ingress.ip_address}"
  type   = "A"
  ttl    = 10
}
