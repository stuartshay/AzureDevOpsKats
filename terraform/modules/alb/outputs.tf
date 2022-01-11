output "target_group_arn" {
  description = "The ALB target group arn"
  value       = aws_lb_target_group.this.arn
}

output "dns_name" {
  description = "The ALB dns name"
  value       = aws_lb.this.dns_name
}
