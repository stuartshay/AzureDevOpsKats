## AWS ECS CLI

https://awscli.amazonaws.com/v2/documentation/api/latest/reference/ecs/index.html

Install AWS CLI 2.0

```
https://docs.aws.amazon.com/cli/latest/userguide/getting-started-install.html
```

Install AWS Session Manager

```
https://docs.aws.amazon.com/systems-manager/latest/userguide/session-manager-working-with-install-plugin.html
```

- List Clusters<br/>
  https://docs.aws.amazon.com/cli/latest/reference/ecs/list-clusters.html

```
aws ecs list-clusters
```

- List Services<br/>

```
aws ecs list-services --cluster <cluster>
aws ecs list-services --cluster devopskats-master
```

- List Tasks<br/>
  https://awscli.amazonaws.com/v2/documentation/api/latest/reference/ecs/list-tasks.html

```
aws ecs list-tasks --cluster <cluster>
aws ecs list-tasks --cluster devopskats-master
```

## Amazon ECS Exec to access your containers

```
aws ecs execute-command  \
    --cluster <cluster> \
    --task <task> \
    --container <container> \
    --command "bash" \
    --interactive

aws ecs execute-command  --cluster devopskats-master --task 2e89b6e86eb247c49906f27eecaeca2e --container devopskats --command "bash" --interactive
```
