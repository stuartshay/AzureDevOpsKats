locals {
  name                    = "devops"
  project_name            = "devopskats"
  lambda_function_name    = "auto-remove-ecs-service"
  lambda_function_runtime = "python3.8"
  lambda_function_mem     = 10240 #10Gb
  lambda_function_timeout = 900   #15 minutes

  envs = [
    "develop",
    "master"
  ]
}

data "terraform_remote_state" "jumpbox" {
  backend = "remote"

  config = {
    organization = "DevOpsKats"
    workspaces = {
      name = "AWSDevOpsKats-Jumpbox"
    }
  }
}

module "ecs" {
  count = length(local.envs)

  source = "./modules/ecs"

  name           = "${local.envs[count.index]}-${local.name}"
  log_group_name = "devopskats-${local.envs[count.index]}"

  execution_role_arn = aws_iam_role.ecs_task_execution_role.arn
  task_role_arn      = aws_iam_role.container.arn

  tags = {
    Env = "${local.envs[count.index]}"
  }
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
  version = "2.34.0"

  create_function = false

  runtime = "python3"
  source_path = [
    "./function/lambda_function.py",
    {
      pip_requirements = "./function/requirements.txt"
    }
  ]

  build_in_docker = false
}


## Deploy from packaged
module "lambda_function" {
  source  = "terraform-aws-modules/lambda/aws"
  version = "2.34.0"

  create_package                    = false
  local_existing_package            = module.package_in_docker.local_filename
  depends_on                        = [module.package_in_docker]
  cloudwatch_logs_retention_in_days = 7

  ignore_source_code_hash = true

  function_name = local.lambda_function_name
  handler       = "lambda_function.lambda_handler"
  runtime       = local.lambda_function_runtime

  publish = false

  memory_size = local.lambda_function_mem
  timeout     = local.lambda_function_timeout

  environment_variables = {
    ECS_CLUSTERS       = "develop-${local.name}"
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

  tags = {
    Env = "develop"
  }
}

## Cloudwatch Events -  Schedule to run lambda function every hour
resource "aws_cloudwatch_event_rule" "hourly" {
  name                = "${local.lambda_function_name}-hourly"
  description         = "Process function hourly"
  schedule_expression = "cron(0 * * * ? *)"

  tags = {
    Env = "develop"
  }
}

resource "aws_cloudwatch_event_target" "lambda_function" {
  rule = aws_cloudwatch_event_rule.hourly.name
  arn  = module.lambda_function.lambda_function_arn
}

## Loadblancing

resource "aws_security_group" "master_alb" {
  name   = "master-${local.name}-alb"
  vpc_id = data.aws_vpc.default.id

  ingress {
    protocol         = "tcp"
    from_port        = 80
    to_port          = 80
    cidr_blocks      = ["0.0.0.0/0"]
    ipv6_cidr_blocks = ["::/0"]
  }

  ingress {
    protocol         = "tcp"
    from_port        = 443
    to_port          = 443
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

resource "aws_security_group" "master_ecs_tasks" {
  name   = "master-${local.name}-ecs-tasks"
  vpc_id = data.aws_vpc.default.id

  ingress {
    protocol        = "tcp"
    from_port       = 5000
    to_port         = 5000
    security_groups = [aws_security_group.master_alb.id]
  }

  egress {
    protocol         = "-1"
    from_port        = 0
    to_port          = 0
    cidr_blocks      = ["0.0.0.0/0"]
    ipv6_cidr_blocks = ["::/0"]
  }
}

module "alb_master" {
  source = "./modules/alb"

  name               = "master-${local.name}"
  vpc_id             = data.aws_vpc.default.id
  subnet_ids         = data.aws_subnets.public.ids
  security_group_ids = [aws_security_group.master_alb.id]
  enable_https       = false

  tags = {
    Env = "master"
  }
}

#EFS 
resource "aws_efs_file_system" "master" {
  creation_token = "master-${local.name}"

  tags = {
    Env  = "master"
    Name = "master-${local.name}"
  }
}

resource "aws_efs_file_system" "develop" {
  creation_token = "develop-${local.name}"

  tags = {
    Env  = "develop"
    Name = "develop-${local.name}"
  }
}

resource "aws_efs_mount_target" "develop" {
  file_system_id  = aws_efs_file_system.develop.id
  subnet_id       = data.aws_subnets.public.ids[0]
  security_groups = [aws_security_group.efs.id]
}

resource "aws_efs_mount_target" "master" {
  file_system_id  = aws_efs_file_system.master.id
  subnet_id       = data.aws_subnets.public.ids[0]
  security_groups = [aws_security_group.efs.id]
}

resource "aws_security_group" "efs" {
  name   = "${local.name}-efs"
  vpc_id = data.aws_vpc.default.id

  ingress {
    protocol  = "tcp"
    from_port = 2049
    to_port   = 2049
    security_groups = [
      aws_security_group.master_ecs_tasks.id,
      aws_security_group.ecs_tasks.id,
      data.terraform_remote_state.jumpbox.outputs.security_group_id
    ]
  }

  egress {
    protocol         = "-1"
    from_port        = 0
    to_port          = 0
    cidr_blocks      = ["0.0.0.0/0"]
    ipv6_cidr_blocks = ["::/0"]
  }
}

