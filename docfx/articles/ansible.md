# Jumpbox Workstation

[![Terraform Deploy Jumpbox Infrastructure](https://github.com/stuartshay/AzureDevOpsKats/actions/workflows/deploy-jumpbox-infra.yml/badge.svg)](https://github.com/stuartshay/AzureDevOpsKats/actions/workflows/deploy-jumpbox-infra.yml)

## Prerequisites

- aws cli
- ansible

Configure AWS Profile

~/.aws/credentials

```
[awsdevopskats]
aws_access_key_id = <AWS_ACCESS_KEY_ID>
aws_secret_access_key = <AWS_SECRET_ACCESS_KEY>
```

Add SSH Private Key

```
chmod 600 ~/.ssh/awsdevopskats.cer
```

## Functions:

- Installing Vscode, nfs-utils, AWS-CLI, Docker, docker-compose, VNC-server
- Setup Ubuntu Desktop mode
- Automation mount EFS master and develop

## How to auto config jumpbox-server

### Preparing

- We need AWS credentials already setup on your env

- Sine we are using ansible aws EC2 auto inventory, so need to update something on this file `ansible/inventory_aws_ec2.yml`

  ```yaml
  filters:
    instance-state-name: running
    tag:application: devopskats
    tag:env: development
    tag:Env: develop
    tag:owner: culiops
    tag:Name: devops-jumpbox
  ```

  Double check ansible hosts after change

  ```bash
  ansible-inventory --graph -i ansible/jumpbox/inventory_aws_ec2.yml
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
ansible-playbook ansible/tasks.yaml -u ubuntu -i ansible/jumpbox/inventory_aws_ec2.yml --private-key ~/.ssh/awsdevopskats.cer --check --diff
```
