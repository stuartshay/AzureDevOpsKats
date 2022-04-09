{
    "cpu": "1024",
    "memory": "1024",
    "containerDefinitions": [
        {
            "name": "nginx",
            "image": "{{ must_env `AWS_ACCOUNT_ID` }}.dkr.ecr.{{ must_env `AWS_REGION` }}.amazonaws.com/tls-sidecar:latest",
            "cpu": 256,
            "essential": true,
            "portMappings": [
                {
                "containerPort": 443,
                "protocol": "tcp"
                }
            ],
            "environment": [
                {
                  "name": "BACKEND_NAME",
                  "value": "localhost"
                },
                {
                  "name": "BACKEND_PORT",
                  "value": "5000"
                }
            ]
        },
        {
            "cpu": 512,
            "essential": true,
            "image": "{{ must_env `AWS_ACCOUNT_ID` }}.dkr.ecr.{{ must_env `AWS_REGION` }}.amazonaws.com/{{ must_env `AWS_ECR_REPOSITORY` }}:{{ must_env `AWS_ECR_DOCKER_IMAGE_TAG` }}",
            "name": "devopskats",
            "dockerLabels": {
              "APPLICATION": "devopskats.web",
              "BUILD_DATE" : "2022-01-01 00:00:00Z",
              "AWS_ECR_DOCKER_IMAGE_TAG" : "{{ must_env `AWS_ECR_DOCKER_IMAGE_TAG` }}"
            },
            "portMappings": [
                {
                    "containerPort": 5000,
                    "protocol": "tcp"
                }
            ],
            "mountPoints": [
                {
                    "sourceVolume": "efs-{{ must_env `BRANCH_NAME` }}",
                    "containerPath": "/images"
                }
            ],
            "logConfiguration": {
                "logDriver": "awslogs",
                "options": {
                    "awslogs-group": "devopskats-{{ must_env `BRANCH_NAME` }}",
                    "awslogs-region": "{{ must_env `AWS_REGION` }}",
                    "awslogs-stream-prefix": "devopskats"
                }
            },
            "environment": [
                {
                  "name": "ASPNETCORE_ENVIRONMENT",
                  "value": "AwsEcs"
                },
                {
                    "name": "CLUSTER_NAME",
                    "value": "{{ must_env `BRANCH_NAME` }}"
                },
                {
                    "name": "SYSTEMS_MANAGER_RELOAD",
                    "value": if std.extVar('branch_name') == "master" then "360" else "60"
                }
            ],
            "secrets": [
                {
                "name": "SMTP_PASSWORD",
                "valueFrom": "arn:aws:ssm:{{ must_env `AWS_REGION` }}:816939196156:parameter/devopskats-{{ must_env `BRANCH_NAME` }}/smtp/password"
                },
                {
                "name": "SMTP_PORT",
                "valueFrom": "arn:aws:ssm:{{ must_env `AWS_REGION` }}:816939196156:parameter/devopskats-{{ must_env `BRANCH_NAME` }}/smtp/port"
                },
                {
                "name": "SMTP_SERVER",
                "valueFrom": "arn:aws:ssm:{{ must_env `AWS_REGION` }}:816939196156:parameter/devopskats-{{ must_env `BRANCH_NAME` }}/smtp/server"
                },
                {
                "name": "SMTP_USERNAME",
                "valueFrom": "arn:aws:ssm:{{ must_env `AWS_REGION` }}:816939196156:parameter/devopskats-{{ must_env `BRANCH_NAME` }}/smtp/username"
                }
            ]
        }
    ],
    "volumes": [
        {
            "name": "efs-{{ must_env `BRANCH_NAME` }}",
            "efsVolumeConfiguration": {
                "fileSystemId": "{{ tfstate `module.efs.aws_efs_file_system.this.id` }}"
            }
        }
    ],
    "family": "{{ must_env `ECS_SERVICE` }}",
    "networkMode": "awsvpc",
    "placementConstraints": [],
    "executionRoleArn": "{{ tfstate `data.terraform_remote_state.shared.outputs.ecs_task_execution_role_arn` }}",
    "taskRoleArn": "{{ tfstate `data.terraform_remote_state.shared.outputs.ecs_container_role_arn` }}",
    "Tags": [
        {
            "Key": "Env",
            "Value": "{{ must_env `BRANCH_NAME` }}"
        }
    ]
}
