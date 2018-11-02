#!/bin/bash


az acs kubernetes get-credentials \
    --ssh-key-file arm_release/key/ssh-key \
    --resource-group=AzureDevOpsKatsGroup \
    --name=containerservice-AzureDevOpsKatsGroup


kubectl get nodes

kubectl create -f azuredevopskats.yaml
