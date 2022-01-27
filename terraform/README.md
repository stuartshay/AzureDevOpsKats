# The github workflow deploy ECS tasks

This is a Terraform code to manage all AWS resources for Github workflow deploy ECS tasks.

## The Architecture Design
![Alt text](docs/Github_Action_ECS.png?raw=true "The Architecture Design")

## The Features

- Deploy ECS clusters for `master` and `develop` branchs
- Deploy ALB for `master` branch, for saving cost, we don't need to make deploy ALB for `develop` branch
- Deploy Lambda function to automatically remove `develop` ECS cluster tasks after 6 hours
- Deploy Cloudwatch Event to schedule calling Lambda function
- Create IAM user/group/role/policy and Security groups
- Automatic deploy ECS services tasks base on branch

## How to deploy AWS resources
```
terraform init
terraform plan
terraform deploy
```

## How to deploy ECS service tasks
- We are using `ecspresso`(https://github.com/kayac/ecspresso) to deploy ECS service tasks.
- We have supported auto deploy ECS tasks only 2 branchs, `develop` and `master`
- For the ecspresso code, please refer to `../ecspresso/ecspresso.yml`
- For some reasons, we using `jsonnet`(https://jsonnet.org/) to control which branch ECS tasks need to have a Load Blancing

## How to auto config jumpbox-server

### Preparing
- We need AWS credentials already setup on your env

- Sine we are using ansible aws EC2 auto inventory, so need to update something on this file `ansible/inventory_aws_ec2.yml`

    ```yaml
    filters:
        instance-state-name : running
        tag:application: devopskats
        tag:env: development
        tag:Env: develop
        tag:owner: culiops
        tag:Name: devops-jumpbox
    ```

    Double check ansible hosts after change

    ```bash
    ansible-inventory --graph -i ansible/inventory_aws_ec2.yml
    ```
    The output of above command:
    ```bash
    @all:
    |--@aws_ec2:
    |  |--ec2-13-229-40-130.ap-southeast-1.compute.amazonaws.com
    |--@ungrouped:
    ```

### Command
```bash
ansible-playbook ansible/tasks.yml -u ubuntu -i ansible/inventory_aws_ec2.yml
```

### Example
```bash
ansible-playbook ansible/tasks.yaml -u ubuntu -i ansible/inventory_aws_ec2.yml --private-key ../../../projects/StuartShay_29111502/culiops.cer --check --diff
```

## The IAM resources
- We have followed best practice during create IAM resources
- We created 1 AWS user and group for Github Workflow using during call AWS services with these policies
- We created 2 roles for ECS task and agent. Actually we don't use it for now, but in the future when ECS tasks or Agent need to access to other AWS resources, it need to be update on these roles.
- Detail, please refer to `iam.tf` file

## The Security Groups
- We have 1 security group for `develop` branch, these ECS tasks will not register to ALB, so security group will only open `5000` port.
- We have 2 security groups for `master` branch, these branch ECS tasks will register to ALB, so one allow all port `80` and `443` to ALB, and one only allow port `5000` from ALB to ECS tasks.

## The Lambda and Cloudwatch Event
- We are using Lambda to manage the `develop` branch ECS tasks, we assume that after 6 hours, all tasks will be deleted automaticly by Lambda function. For how long to remove `develop` ECS tasks, can config `REMOVE_AFTER_HOURS` from `main.tf` file.
    ```
        environment_variables = {
            ECS_CLUSTERS       = "develop-${local.name}"
            REMOVE_AFTER_HOURS = 6
        }
    ```
- We are using Cloudwatch Event to schedule to call Lambda function, currently every hours, to config this please refer this config on `main.tf` file
    ```
    resource "aws_cloudwatch_event_rule" "hourly" {
        name                = "${local.lambda_function_name}-hourly"
        description         = "Process function hourly"
        schedule_expression = "cron(0 * * * ? *)"
    }
    ```
- The Lambda function is using `Python` and `Boto3` library
- The Terraform code already help to build, package and push the code to Lambda function automatically, so basicly nothing much to do for deployment it.
- We can config how much Memory for function and how long for timeout bu config these things on `main.tf`
    ```hcl
    lambda_function_mem     = 10240 #10Gb
    lambda_function_timeout = 900   #15 minutes
    ```

## The Monitoring -  Cloudwatch
- ALB metrics: Total requests, heathly tasks, unhealthy tasks, response time
- ECS metrics: CPU, Memory utilization of cluster
- Lambda metrics: Invocations total, Errors total, Duration, ConcurrentExecutions

## The AWS Cost
- We asumme that, 1 ECS task will need `1GB Memory` and `0.5 CPU`, for avegate `2 tasks/day` for `master` ==> `43 USD/Month`, and `1 task/6 hours` for `develop` ==> `15 USD/Month` ===> `55-60 USD/Month`
- Lambda function: `24 requests/day`, `duration = 2 minutes`, `2GB Memory` ===> `3 USD/Month`
- ALB: we have `25 USD/Month` for ALB month cost, depend how much requests we call ALB, with `100 requests/second` and `10 new connections/second` ===> `10 USD/Month`
- Cloudwatch Dashboard: `3 USD/Dashboard/Month`

## Note
- Need to create AWS resources by using Terraform before do something with Github workflow

## References
- Ecspresso: https://github.com/kayac/ecspresso
- Jsonnet: https://jsonnet.org/
- Terraform: https://www.terraform.io/