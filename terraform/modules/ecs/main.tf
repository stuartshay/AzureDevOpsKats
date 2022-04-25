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

resource "aws_ecs_service" "this" {
  name            = var.name
  cluster         = module.ecs.ecs_cluster_name
  task_definition = aws_ecs_task_definition.this.arn
  desired_count   = 0

  network_configuration {
    subnets          = var.subnets
    security_groups  = var.security_group_ids
    assign_public_ip = true
  }

  dynamic "load_balancer" {
    for_each = var.load_balancers
    content {
      target_group_arn = load_balancer.value["target_group_arn"]
      container_name   = load_balancer.value["container_name"]
      container_port   = load_balancer.value["container_port"]
    }
  }

  lifecycle {
    ignore_changes = [desired_count, task_definition, capacity_provider_strategy, ordered_placement_strategy, deployment_minimum_healthy_percent]
  }

  depends_on = [
    module.ecs
  ]
}
