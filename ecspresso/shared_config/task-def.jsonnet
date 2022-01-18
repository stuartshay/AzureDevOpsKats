{
    "cpu": "512",
    "memory": "1024",
    "containerDefinitions": [
        {
            "cpu": 512,
            "essential": true,
            "image": "{{ must_env `AWS_ACCOUNT_ID` }}.dkr.ecr.{{ must_env `AWS_REGION` }}.amazonaws.com/{{ must_env `AWS_ECR_REPOSITORY` }}:{{ must_env `AWS_ECR_DOCKER_IMAGE_TAG` }}",
            "name": "devopskats",
            "portMappings": [
                {
                    "containerPort": 5000,
                    "protocol": "tcp"
                }
            ],
            "volumesFrom": [],
            "logConfiguration": {
                "logDriver": "awslogs",
                "options": {
                    "awslogs-group": "devopskats",
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
                    "value": if std.extVar('branch_name') == "master" then 360 else 60
                }
            ]
        }
    ],
    "family": "{{ must_env `ECS_SERVICE` }}",
    "networkMode": "awsvpc",
    "placementConstraints": [],
    "executionRoleArn": "{{ tfstate `aws_iam_role.ecs_task_execution_role.arn` }}",
    "taskRoleArn": "{{ tfstate `aws_iam_role.container.arn` }}"
}