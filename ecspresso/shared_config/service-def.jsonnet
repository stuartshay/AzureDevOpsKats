{
    "capacityProviderStrategy": [
        {
            "base": 0,
            "capacityProvider": "FARGATE_SPOT",
            "weight": 1
        }
    ],
    "deploymentConfiguration": {
        "deploymentCircuitBreaker": {
            "enable": false,
            "rollback": false
        },
        "maximumPercent": 100,
        "minimumHealthyPercent": 0
    },
    "NetworkConfiguration": {
        "AwsvpcConfiguration": {
            "AssignPublicIp": "ENABLED",
            "SecurityGroups": [
                "{{ tfstate `module.security_group_ecs_tasks.aws_security_group.this.id` }}"
            ],
            "Subnets": [
                "{{ tfstate `data.terraform_remote_state.network.outputs.public_subnet_ids[0]` }}",
                "{{ tfstate `data.terraform_remote_state.network.outputs.public_subnet_ids[1]` }}",
                "{{ tfstate `data.terraform_remote_state.network.outputs.public_subnet_ids[2]` }}"
            ]
        }
    },
    "loadBalancers": [
    ] + (
        if std.extVar('branch_name') == "master" then [
            { "targetGroupArn": "{{ tfstate `module.alb.aws_lb_target_group.this.arn` }}", "containerName": "nginx", "containerPort": 80 },
        ] else []
    ),
    "enableECSManagedTags": true,
    "enableExecuteCommand": true,
    "placementConstraints": [],
    "placementStrategy": [],
    "schedulingStrategy": "REPLICA",
    "serviceRegistries": [],
    "desiredCount": std.extVar('desired_count'),
    "Tags": [
        {
            "Key": "Env",
            "Value": "{{ must_env `BRANCH_NAME` }}"
        }
    ],
    "propagateTags": "TASK_DEFINITION"
}