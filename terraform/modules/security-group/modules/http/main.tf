resource "aws_security_group" "this" {
  name        = var.name
  description = var.name

  vpc_id = var.vpc_id

  egress {
    from_port   = 0
    to_port     = 0
    protocol    = "-1"
    cidr_blocks = ["0.0.0.0/0"]
  }
}

resource "aws_security_group_rule" "cidr_ingresses_http" {
  count = length(var.cidr_ingresses) > 0 ? 1 : 0

  security_group_id = aws_security_group.this.id

  type        = "ingress"
  from_port   = 80
  to_port     = 80
  protocol    = "TCP"
  cidr_blocks = var.cidr_ingresses
}

resource "aws_security_group_rule" "cidr_ingresses_https" {
  count = length(var.cidr_ingresses) > 0 ? 1 : 0

  security_group_id = aws_security_group.this.id

  type        = "ingress"
  from_port   = 443
  to_port     = 443
  protocol    = "TCP"
  cidr_blocks = var.cidr_ingresses
}

resource "aws_security_group_rule" "sg_ingresses_http" {
  count = length(var.sg_ingresses)

  security_group_id = aws_security_group.this.id

  type                     = "ingress"
  from_port                = 80
  to_port                  = 80
  protocol                 = "TCP"
  source_security_group_id = var.sg_ingresses[count.index]
}

resource "aws_security_group_rule" "sg_ingresses_https" {
  count = length(var.sg_ingresses)

  security_group_id = aws_security_group.this.id

  type                     = "ingress"
  from_port                = 443
  to_port                  = 443
  protocol                 = "TCP"
  source_security_group_id = var.sg_ingresses[count.index]
}
