module "cosmosdb" {
  count                            = var.database_engine == "CosmosDb" ? 1 : 0
  source                           = "./modules/cosmosdb"
  name                             = "${var.resource_prefix}-cosmos"
  location                         = var.location
  resource_group_name              = azurerm_resource_group.rg.name
  tags                             = var.tags
  databases = {
    "Catalog" = {
      scale = {
        autoscale      = true
        max_throughput = 1000
      }
    }
    "Identity" = {
      scale = {
        autoscale      = true
        max_throughput = 1000
      }
    }
  }
}
