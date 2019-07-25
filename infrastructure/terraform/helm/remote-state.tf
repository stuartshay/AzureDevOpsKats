data "terraform_remote_state" "main" {
  backend = "azurerm"
  config = {
    resource_group_name  = "Terraform"
    storage_account_name = "terraformstatestor"
    container_name       = "azure-devops"
    key                  = "AzureDevOpsKats/AzureContainerServices/pipelines.terraform.tfstate"
    access_key           = "0FUUmrDfreZ8sm/tQooZKJ2fYXpCEh6hMjL18BrpKdnDdy1D229RS94KZ1m7ja3CfG2V8CWVYq6nEQds9Kq2lg=="
  }
}
