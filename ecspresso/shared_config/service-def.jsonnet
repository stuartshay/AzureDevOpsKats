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
                "{{ tfstate `aws_security_group.ecs_tasks.id` }}"
            ],
            "Subnets": [
                "{{ tfstate `data.aws_subnets.public.ids[0]` }}",
                "{{ tfstate `data.aws_subnets.public.ids[1]` }}"
            ]
        }
    },
    "loadBalancers": [
        { "targetGroupArn": "{{ tfstate `module.alb_master.aws_lb_target_group.this.arn` }}", "containerName": "devopskats", "containerPort": 5000 }
    ],
    "enableECSManagedTags": true,
    "enableExecuteCommand": true,
    "placementConstraints": [],
    "placementStrategy": [],
    "schedulingStrategy": "REPLICA",
    "serviceRegistries": [],
    "desiredCount": 1
}