resource "aws_iam_role" "ecs_task_execution_role" {
  name = "devopskats-ecsTaskExecutionRole"

  assume_role_policy = <<EOF
{
 "Version": "2012-10-17",
 "Statement": [
   {
     "Action": "sts:AssumeRole",
     "Principal": {
       "Service": "ecs-tasks.amazonaws.com"
     },
     "Effect": "Allow",
     "Sid": ""
   }
 ]
}
EOF
}

resource "aws_iam_role_policy_attachment" "ecs-task-execution-role-policy-attachment" {
  role       = aws_iam_role.ecs_task_execution_role.name
  policy_arn = "arn:aws:iam::aws:policy/service-role/AmazonECSTaskExecutionRolePolicy"
}


resource "aws_iam_role" "deploy_user" {
  name = "devopskats-deploy"

  assume_role_policy = jsonencode({
    Version = "2012-10-17",
    Statement = [
      {
        Action = "sts:AssumeRole"
        Principal = {
          Service = "codebuild.amazonaws.com"
        }
        Effect = "Allow"
        Sid    = ""
      }
    ]
  })
}

module "deploy_user" {
  source  = "terraform-aws-modules/iam/aws//modules/iam-user"
  version = "~> 4.0"

  name          = "giithub-action-deploy-ecs-deploy"
  force_destroy = true

  create_iam_user_login_profile = false
  create_iam_access_key         = true
}

data "aws_iam_policy_document" "deploy_user" {
  statement {
    actions = [
      "ecr:BatchCheckLayerAvailability",
      "ecr:BatchGetImage",
      "ecr:GetDownloadUrlForLayer",
      "ecr:GetAuthorizationToken",
      "ecr:InitiateLayerUpload",
      "ecr:UploadLayerPart",
      "ecr:CompleteLayerUpload",
      "ecr:DescribeImages",
      "ecr:PutImage",
      "ecs:RegisterTaskDefinition",
      "ecs:CreateService",
      "ecs:UpdateService",
      "ecs:RegisterTaskDefinition",
      "ecs:RunTask",
      "ecs:StartTask",
      "ecs:StopTask",
      "ecs:Describe*",
      "ecs:List*",
      "ec2:DescribeNetworkInterfaces",
      "application-autoscaling:*",
      "logs:CreateLogStream",
      "logs:PutLogEvents",
      "elasticloadbalancing:DescribeTargetGroups"
    ]

    resources = [
      "*"
    ]
  }

  statement {
    actions = [
      "iam:GetRole",
      "iam:PassRole"
    ]

    resources = [
      aws_iam_role.ecs_task_execution_role.arn,
      aws_iam_role.container.arn
    ]
  }

  statement {
    actions = [
      "s3:GetObject"
    ]

    resources = [
      "arn:aws:s3:::devops-team-tfstate/*"
    ]
  }
}

module "deploy_group" {
  source  = "terraform-aws-modules/iam/aws//modules/iam-group-with-policies"
  version = "~> 4.0"

  name = "deploy_group"

  group_users = [
    module.deploy_user.iam_user_name
  ]

  attach_iam_self_management_policy = false
  custom_group_policies = [
    {
      name   = "AllowECSDeployment"
      policy = data.aws_iam_policy_document.deploy_user.json
    },
  ]
}


resource "aws_iam_role" "container" {
  name = "devopskats-container"
  assume_role_policy = jsonencode({
    Version = "2012-10-17"
    Statement = [
      {
        Action = "sts:AssumeRole"
        Effect = "Allow"
        Sid    = "ecsTasks"
        Principal = {
          Service = "ecs-tasks.amazonaws.com"
        }
      },
    ]
  })
}


resource "aws_iam_role_policy" "container" {
  name = "devopskats-container"
  role = aws_iam_role.container.id
  policy = jsonencode({
    Version = "2012-10-17"
    Statement = [
      {
        # For ecs exec
        Effect = "Allow"
        Action = [
          "ssmmessages:CreateControlChannel",
          "ssmmessages:CreateDataChannel",
          "ssmmessages:OpenControlChannel",
          "ssmmessages:OpenDataChannel"
        ]
        Resource = "*"
      }
    ]
  })
}
