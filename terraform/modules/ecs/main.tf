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

  tags = var.tags
}

resource "aws_cloudwatch_log_group" "this" {
  name              = var.log_group_name
  retention_in_days = 7
  tags              = var.tags
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
    "name": "${var.name}-dummy",
    "image": "nginx",
    "cpu": 0,
    "memory": 128
  }
]
EOF

  tags = var.tags
}
