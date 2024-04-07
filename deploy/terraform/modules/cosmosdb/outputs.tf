output "conection_string" {
    description = "Connection string for the Cosmos Account created."
    value       = azurerm_cosmosdb_account.account.primary_sql_connection_string
    sensitive   = true
}
