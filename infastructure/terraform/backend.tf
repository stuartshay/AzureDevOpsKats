terraform {
  backend "azurerm" {
    resource_group_name  = "Terraform"
    storage_account_name = "terraformstatestor"
    container_name       = "azure-devops"
    key                  = "AzureDevOpsKats/AzureContainerServices/pipelines.terraform.tfstate"
  }
}
