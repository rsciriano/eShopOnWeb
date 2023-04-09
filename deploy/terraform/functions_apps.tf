data "archive_file" "function" {
  type        = "zip"
  source_dir  = var.function_app_publish_dir
  output_path = "${var.function_app_package_dir}/functions_${var.function_app_version}.zip"

}

module "funtions_app" {
  depends_on = [
    data.archive_file.function,
    module.servicebus
  ]
  source                     = "./modules/functions_app"
  project                    = var.resource_prefix
  environment                = var.environment
  location                   = var.location
  resource_group_name        = azurerm_resource_group.rg.name
  tags                       = var.tags

  storage_account_name       = module.storage_account.name
  storage_account_access_key = module.storage_account.primary_access_key

  zip_deploy_file            = "${var.function_app_package_dir}/functions_${var.function_app_version}.zip"

  app_settings = {
    "APPINSIGHTS_INSTRUMENTATIONKEY"         = module.application_insights.instrumentation_key
    "APPLICATIONINSIGHTS_CONNECTION_STRING"  = module.application_insights.connection_string
    "FUNCTIONS_WORKER_RUNTIME"               = "dotnet-isolated"
    "ServiceBusConnection"                   = module.servicebus.conection_string
  }



  connection_strings = {
    "ServiceBusConnection" = {
      type    = "ServiceBus"
      value   = module.servicebus.conection_string
    },
    "CatalogConnection" = {
      type    = "SQLAzure"
      value   = module.mssql.conection_strings.Catalog
    }
  }
}