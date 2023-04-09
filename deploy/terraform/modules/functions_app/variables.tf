
variable "project" {
  description = "(Required) Specifies the name of the project"
  type        = string
}

variable "environment" {
  description = "(Required) Specifies the environment name"
  type        = string
}

variable "resource_group_name" {
  description = "(Required) Specifies the resource group name"
  type = string
}

variable "storage_account_name" {
  type = string
}

variable "storage_account_access_key" {
  type = string
}

variable "tags" {
  description = "(Optional) Specifies the tags of the log analytics workspace"
  type        = map(any)
  default     = {}
}

variable "location" {
  description = "(Required) Specifies the supported Azure location where the resource exists. Changing this forces a new resource to be created."
  type        = string
}

variable "zip_deploy_file" {
  type = string
}

variable "app_settings" {
  type = map(any)
  default     = {}
}

variable "connection_strings" {
  type = map(object({
    type  = string # Possible values are APIHub, Custom, DocDb, EventHub, MySQL, NotificationHub, PostgreSQL, RedisCache, ServiceBus, SQLAzure and SQLServer
    value = string
  }))
  default     = {}
}