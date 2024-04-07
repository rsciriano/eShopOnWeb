resource "azurerm_key_vault" "kv" {
  name                = "${var.resource_prefix}-kv"
  resource_group_name = azurerm_resource_group.rg.name
  tenant_id           = data.azurerm_client_config.current.tenant_id
  location            = azurerm_resource_group.rg.location
  sku_name            = var.kv_sku_name
  access_policy {
    tenant_id         = data.azurerm_client_config.current.tenant_id
    object_id         = data.azurerm_client_config.current.object_id

    secret_permissions = [
      "Set",
      "Get",
      "Delete",
      "Purge",
      "Recover",
      "List"
    ]
  }
}
