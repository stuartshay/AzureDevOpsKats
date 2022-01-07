module "ecs" {
  source = "terraform-aws-modules/ecs/aws"

  name = var.name

  container_insights = false

  capacity_providers = ["FARGATE", "FARGATE_SPOT"]

  default_capacity_provider_strategy = [
    {
      capacity_provider = "FARGATE_SPOT"
      weight            = "1"
    }
  ]

}

resource "aws_cloudwatch_log_group" "this" {
  name              = var.name
  retention_in_days = 7
}

#----- ECS  Services--------
resource "aws_ecs_task_definition" "this" {
  family                   = var.name
  requires_compatibilities = ["FARGATE"]
  network_mode             = "awsvpc"
  cpu                      = 1024
  memory                   = 2048
  execution_role_arn       = var.execution_role_arn
  task_role_arn            = var.task_role_arn

  container_definitions = <<EOF
[
  {
    "name": "devopskats",
    "image": "nginx",
    "cpu": 0,
    "memory": 128
  }
]
EOF
}

# resource "aws_ecs_service" "this" {
#   name            = var.name
#   cluster         = module.ecs.ecs_cluster_id
#   task_definition = aws_ecs_task_definition.this.arn

#   network_configuration {
#     security_groups  = var.security_group_ids
#     subnets          = var.subnet_ids
#     assign_public_ip = true
#   }

#   desired_count = 1

#   deployment_maximum_percent         = 100
#   deployment_minimum_healthy_percent = 0
# }
