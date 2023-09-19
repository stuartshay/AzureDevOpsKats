module "package_in_docker" {
  source  = "terraform-aws-modules/lambda/aws"
  version = "2.36.0"

  create_function = false

  runtime = "python3"
  source_path = [
    "${path.module}/function/lambda_function.py",
    {
      pip_requirements = "${path.module}/function/requirements.txt"
    }
  ]

  build_in_docker = false
}


## Deploy from packaged
module "lambda_function" {
  source  = "terraform-aws-modules/lambda/aws"
  version = "2.36.0"

  create_package                    = false
  local_existing_package            = module.package_in_docker.local_filename
  depends_on                        = [module.package_in_docker]
  cloudwatch_logs_retention_in_days = 7

  ignore_source_code_hash = true

  function_name = var.name
  handler       = "lambda_function.lambda_handler"
  runtime       = var.runtime

  publish = false

  memory_size = var.memory
  timeout     = var.timeout

  environment_variables = {
    ECS_CLUSTERS       = var.ecs_cluster_name
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
  name                = "${var.name}-hourly"
  description         = "Process function hourly"
  schedule_expression = "cron(0 * * * ? *)"

  tags = {
    Env = "develop"
  }
}

resource "aws_cloudwatch_event_target" "hourly" {
  rule = aws_cloudwatch_event_rule.hourly.name
  arn  = module.lambda_function.lambda_function_arn
}