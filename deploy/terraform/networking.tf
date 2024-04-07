module "virtual_network" {
  source                           = "./modules/virtual_network"
  resource_group_name              = azurerm_resource_group.rg.name
  vnet_name                        = "${var.resource_prefix != "" ? var.resource_prefix : random_string.resource_prefix.result}${var.vnet_name}"
  location                         = var.location
  address_space                    = var.vnet_address_space
  tags                             = var.tags
  log_analytics_workspace_id       = module.log_analytics_workspace.id
  log_analytics_retention_days     = var.log_analytics_retention_days

  subnets = [
    {
      name : var.aca_subnet_name
      address_prefixes : var.aca_subnet_address_prefix
      private_endpoint_network_policies_enabled : true
      private_link_service_network_policies_enabled : false
    },
    {
      name : var.private_endpoint_subnet_name
      address_prefixes : var.private_endpoint_subnet_address_prefix
      private_endpoint_network_policies_enabled : true
      private_link_service_network_policies_enabled : false
    }
  ]
}
