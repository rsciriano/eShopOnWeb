terraform {
  required_version = ">= 1.3"
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "~> 3.51.0"
    }
  }
}


resource "azurerm_app_service_plan" "app_service_plan" {
  name                = "${var.project}-${var.environment}-service-plan"
  resource_group_name = var.resource_group_name
  location            = var.location
  kind                = "FunctionApp"
  reserved = false # this has to be set to true for Linux. Not related to the Premium Plan
  sku {
    tier = "Dynamic"
    size = "Y1"
  }
}

resource "azurerm_windows_function_app" "function_app" {
  name                       = "${var.project}-${var.environment}-func"
  resource_group_name        = var.resource_group_name
  location                   = var.location
  
  storage_account_name       = var.storage_account_name
  storage_account_access_key = var.storage_account_access_key
  service_plan_id            = azurerm_app_service_plan.app_service_plan.id

  site_config {
    
  }

  app_settings = merge({
    "WEBSITE_RUN_FROM_PACKAGE" = 1,
  }, var.app_settings)

  dynamic "connection_string" {
    for_each = var.connection_strings

    content {
        name = connection_string.key
        type = connection_string.value.type
        value = connection_string.value.value
    }
  }
  
  zip_deploy_file = var.zip_deploy_file
  
  lifecycle {
    ignore_changes = [
      app_settings["WEBSITE_RUN_FROM_PACKAGE"],
    ]
  }
}