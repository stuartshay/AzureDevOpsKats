resource "aws_iam_role" "ecs_task_execution" {
  name = "${var.name}-ecs-execution"

  assume_role_policy = <<EOF
{
 "Version": "2012-10-17",
 "Statement": [
   {
     "Action": "sts:AssumeRole",
     "Principal": {
       "Service": "ecs-tasks.amazonaws.com"
     },
     "Effect": "Allow"
   }
 ]
}
EOF
}

resource "aws_iam_role_policy_attachment" "ecs_task_execution" {
  role       = aws_iam_role.ecs_task_execution.name
  policy_arn = "arn:aws:iam::aws:policy/service-role/AmazonECSTaskExecutionRolePolicy"
}

resource "aws_iam_role" "github_action" {
  name = "${var.name}-github-action"

  assume_role_policy = jsonencode({
    Version = "2012-10-17",
    Statement = [
      {
        Action = "sts:AssumeRole"
        Principal = {
          Service = "codebuild.amazonaws.com"
        }
        Effect = "Allow"
      }
    ]
  })
}

module "github_action_user" {
  source  = "terraform-aws-modules/iam/aws//modules/iam-user"
  version = "~> 4.0"

  name          = "${var.name}-github-action"
  force_destroy = true

  create_iam_user_login_profile = false
  create_iam_access_key         = true
}

data "aws_iam_policy_document" "github_action" {
  statement {
    actions = [
      "iam:DetachRolePolicy",
      "iam:Get*",
      "iam:List*",
      "iam:DeletePolicy",
      "iam:DeleteRole",
      "iam:CreateRole",
      "iam:CreatePolicy",
      "iam:AttachRolePolicy",
      "iam:TagRole",
      "iam:TagPolicy",
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
      "elasticloadbalancing:DescribeTargetGroups",
      "elasticloadbalancing:DescribeLoadBalancers",
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
      aws_iam_role.ecs_task_execution.arn,
      aws_iam_role.ecs_container.arn
    ]
  }

  statement {
    actions = [
      "s3:GetObject",
      "s3:PutObject"
    ]

    resources = [
      "arn:aws:s3:::devops-team-tfstate/*"
    ]
  }

  statement {
    actions = [
      "ssm:GetParameters",
      "ssm:GetParameterHistory",
      "ssm:GetParametersByPath",
      "ssm:GetParameter"
    ]

    resources = [
      "arn:aws:ssm:us-east-1:816939196156:parameter/devopskats/*"
    ]
  }

}

module "github_action_group" {
  source  = "terraform-aws-modules/iam/aws//modules/iam-group-with-policies"
  version = "~> 4.0"

  name = "${var.name}-github-action-group"

  group_users = [
    module.github_action_user.iam_user_name
  ]

  attach_iam_self_management_policy = false
  custom_group_policies = [
    {
      name   = "AllowECSDeployment"
      policy = data.aws_iam_policy_document.github_action.json
    },
  ]

  custom_group_policy_arns = [
    "arn:aws:iam::aws:policy/AmazonEC2FullAccess",
    "arn:aws:iam::aws:policy/AmazonElasticFileSystemFullAccess",
    "arn:aws:iam::aws:policy/AmazonSSMFullAccess",
    "arn:aws:iam::aws:policy/AWSLambda_FullAccess",
    "arn:aws:iam::aws:policy/AmazonECS_FullAccess",
    "arn:aws:iam::aws:policy/AmazonEventBridgeFullAccess"
  ]
}

resource "aws_iam_role" "ecs_container" {
  name = "${var.name}-ecs-container"
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


resource "aws_iam_role_policy" "ecs_container" {
  name = "${var.name}-ecs-container"
  role = aws_iam_role.ecs_container.id
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
          "ssmmessages:OpenDataChannel",
          "logs:CreateLogGroup",
          "logs:CreateLogStream",
          "logs:PutLogEvents",
          "logs:DescribeLogGroups"
        ]
        Resource = "*"
      },
      {
        Effect = "Allow"
        Action = [
          "ssm:GetParameters",
          "ssm:GetParameterHistory",
          "ssm:GetParametersByPath",
          "ssm:GetParameter"
        ]
        Resource = "*"
      }
    ]
  })
}


resource "aws_iam_role_policy" "ecs_execution_ssm" {
  name = "${var.name}-ecs-execution-ssm"
  role = aws_iam_role.ecs_task_execution.id
  policy = jsonencode({
    Version = "2012-10-17"
    Statement = [
      {
        Effect = "Allow"
        Action = [
          "ssm:GetParameters",
          "ssm:GetParameterHistory",
          "ssm:GetParametersByPath",
          "ssm:GetParameter"
        ]
        Resource = [
          "arn:aws:ssm:us-east-1:816939196156:parameter/*"
        ]
      }
    ]
  })
}
