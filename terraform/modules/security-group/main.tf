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

  tags = {
    Name = var.name
  }
}

resource "aws_security_group_rule" "cidr_ingresses" {
  for_each = var.cidr_ingresses

  security_group_id = aws_security_group.this.id

  type        = "ingress"
  from_port   = each.value.from_port
  to_port     = each.value.to_port
  protocol    = each.value.protocol
  cidr_blocks = each.value.cidr_blocks
}

resource "aws_security_group_rule" "sg_ingresses" {
  for_each = var.sg_ingresses

  security_group_id = aws_security_group.this.id

  type                     = "ingress"
  from_port                = each.value.from_port
  to_port                  = each.value.to_port
  protocol                 = each.value.protocol
  source_security_group_id = each.value.security_group_id
}
