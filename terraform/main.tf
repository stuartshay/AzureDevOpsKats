locals {
  name = "devops"
  lambda_function_name = "auto-remove-ecs-service"
  lambda_function_runtime = "python3.8"
  lambda_function_mem     = 10240 #10Gb
  lambda_function_timeout = 900   #15 minutes

  envs = [
    "develop",
    "master"
  ]
}


resource "aws_cloudwatch_log_group" "devopskats" {
  name              = "devopskats"
  retention_in_days = 7
}

module "ecs" {
  count = length(local.envs)

  source = "./modules/ecs"

  name = "${local.envs[count.index]}-${local.name}"

  execution_role_arn = aws_iam_role.ecs_task_execution_role.arn
  task_role_arn      = aws_iam_role.container.arn

}

resource "aws_security_group" "ecs_tasks" {
  name   = "${local.name}-sg-task"
  vpc_id = data.aws_vpc.default.id

  ingress {
    protocol         = "tcp"
    from_port        = 5000
    to_port          = 5000
    cidr_blocks      = ["0.0.0.0/0"]
    ipv6_cidr_blocks = ["::/0"]
  }

  egress {
    protocol         = "-1"
    from_port        = 0
    to_port          = 0
    cidr_blocks      = ["0.0.0.0/0"]
    ipv6_cidr_blocks = ["::/0"]
  }
}

## Build python package with docker image - Some packages will not work if you build on MAC OS
module "package_in_docker" {
  source  = "terraform-aws-modules/lambda/aws"
  version = "2.8.0"

  create_function = false

  runtime = local.lambda_function_runtime
  source_path = [
    "./function/lambda_function.py",
    {
      pip_requirements = "./function/requirements.txt"
    }
  ]

  build_in_docker = true
}


## Deploy from packaged
module "lambda_function" {
  source  = "terraform-aws-modules/lambda/aws"
  version = "2.8.0"

  create_package                    = false
  local_existing_package            = module.package_in_docker.local_filename
  depends_on                        = [module.package_in_docker]
  cloudwatch_logs_retention_in_days = 7

  function_name = local.lambda_function_name
  handler       = "lambda_function.lambda_handler"
  runtime       = local.lambda_function_runtime

  publish = false

  memory_size = local.lambda_function_mem
  timeout     = local.lambda_function_timeout

  environment_variables = {
    ECS_CLUSTERS = "develop-${local.name}"
    REMOVE_AFTER_HOURS = 6
  }

 create_current_version_allowed_triggers = false
  allowed_triggers = {
    OneRule = {
      principal  = "events.amazonaws.com"
      source_arn = aws_cloudwatch_event_rule.hourly.arn
    }
  }

  attach_policy_statements = true
  policy_statements = {
    ecs_delete_service = {
      effect = "Allow",
      actions = [
        "ecs:Describe*",
        "ecs:List*",
        "ecs:UpdateService"
      ],
      resources = [
        "*"
      ]
    },
  }
}

## Cloudwatch Events -  Schedule to run lambda function every hour
resource "aws_cloudwatch_event_rule" "hourly" {
    name = "${local.lambda_function_name}-hourly"
    description = "Process function hourly"
    schedule_expression = "cron(0 * * * ? *)"
}