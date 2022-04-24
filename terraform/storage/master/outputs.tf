output "efs_sg_id" {
  description = "The EFS security group ID"
  value       = module.security_group_efs.id
}

output "efs_id" {
  description = "The EFS security group ID"
  value       = module.efs.id
}
