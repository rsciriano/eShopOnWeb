module "container_apps" {
  depends_on                       = [
    module.mssql, 
    module.cosmosdb
  ]
  source                           = "./modules/container_apps"
  managed_environment_name         = "${var.resource_prefix != "" ? var.resource_prefix : random_string.resource_prefix.result}${var.managed_environment_name}"
  location                         = var.location
  resource_group_name              = azurerm_resource_group.rg.name
  tags                             = var.tags
  infrastructure_subnet_id         = module.virtual_network.subnet_ids[var.aca_subnet_name] 
  instrumentation_key              = module.application_insights.instrumentation_key
  workspace_id                     = module.log_analytics_workspace.id
  dapr_components                  = [{
      name            = var.dapr_name
      component_type  = var.dapr_component_type
      version         = var.dapr_version
      ignore_errors   = var.dapr_ignore_errors
      init_timeout    = var.dapr_init_timeout
      secret          = [
        {
          name        = "storageaccountkey"
          value       = module.storage_account.primary_access_key
        }
      ]
      metadata: [
        {
          name        = "accountName"
          value       = module.storage_account.name
        },
        {
          name        = "containerName"
          value       = var.container_name
        },
        {
          name        = "accountKey"
          secret_name = "storageaccountkey"
        }
      ]
      scopes          = var.dapr_scopes
      }]
  container_apps                   = [{
      name                = "eshop-web"
      revision_mode       = "Single"
      ingress             = {
          external_enabled    = true
          target_port         = 8080
          transport           = "http"
          traffic_weight      = [{
              latest_revision     = true
              percentage          = 100
          }]
      }
      dapr                           = {
          app_id               = "eshop-web"
          app_port             = 8080
          app_protocol         = "http"
      }
      template              = {
        containers            = [{
            name                 = "web"
            image                = var.app_image
            cpu                  = 0.5
            memory               = "1Gi"
            env                  = [{
                name                 = "APPLICATIONINSIGHTS_CONNECTION_STRING"
                secret_name          = "insights-connection"
            },
            {
                name                 = "AZURE_KEY_VAULT_ENDPOINT"
                value                = azurerm_key_vault.kv.vault_uri
            },
            {
                name                 = "ASPNETCORE_URLS"
                value                = "http://+:8080"
            },
            {
                name                 = "DatabaseEngine"
                value                = var.database_engine
            },
            {
                name                 = "ConnectionStrings__CatalogConnection"
                secret_name          = "catalog-connection"
            },
            {
                name                 = "ConnectionStrings__IdentityConnection"
                secret_name          = "identity-connection"
            },
            {
                name                 = "Testing__LoadTestsAuthenticationEnabled"
                value                = true
            }]
        }]
        min_replicas                 = 0
        max_replicas                 = 1
      }
      secrets = [
        {
          name = "insights-connection"
          value = module.application_insights.connection_string
        },
        {
          name = "insights-instrumentationkey"
          value = module.application_insights.instrumentation_key
        },
        {
          name = "catalog-connection"
          value = local.db_connection_strings.Catalog
        },
        {
          name = "identity-connection"          
          value = local.db_connection_strings.Identity
        }
      ]
    }]
}