terraform {
  required_version = ">= 1.3"
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = ">= 3.98.0"
    }
  }
}

provider "azurerm" {
  features {
    resource_group {
      prevent_deletion_if_contains_resources = false
    }
  }
}

data "azurerm_client_config" "current" {
}

resource "random_string" "resource_prefix" {
  length  = 6
  special = false
  upper   = false
  numeric  = false
}

resource "azurerm_resource_group" "rg" {
  name     = "${var.resource_prefix != "" ? var.resource_prefix : random_string.resource_prefix.result}${var.resource_group_name}"
  location = var.location
  tags     = var.tags
}

locals {
  db_connection_strings = {
    Catalog = (
      var.database_engine == "SqlServer" ? module.mssql.conection_strings.Catalog
      : var.database_engine == "CosmosDb" ? module.cosmosdb.conection_string
      :"")
    
    Identity = (
      var.database_engine == "SqlServer" ? module.mssql.conection_strings.Identity
      : var.database_engine == "CosmosDb" ? module.cosmosdb.conection_string
      :"")
  }
}
