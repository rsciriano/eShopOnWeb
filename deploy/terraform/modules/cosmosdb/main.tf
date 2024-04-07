terraform {
  required_version = ">= 1.3"
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "~> 3.43.0"
    }
  }
}

resource "azurerm_cosmosdb_account" "account" {
  name                = var.name
  resource_group_name = var.resource_group_name
  offer_type          = "Standard"
  enable_free_tier    = true
  kind                = "GlobalDocumentDB"
  location            = var.location
  geo_location {
    location          = var.location
    failover_priority = 0
  }
  consistency_policy {
    consistency_level = "Session"
  }
  tags = var.tags
}

resource "azurerm_cosmosdb_sql_database" "db" {
  for_each            = var.databases
  name                = each.key
  resource_group_name = var.resource_group_name
  account_name        = azurerm_cosmosdb_account.account
  throughput          = each.value.scale != null && each.value.scale.autoscale ? null : each.value.throughput
  autoscale_settings  = each.value.scale != null && each.value.autoscale ? {
    max_throughput    = each.value.scale.max_throughput
  } : null
}