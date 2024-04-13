
variable "name" {
  description = "(Required) Specifies the name of the resource."
  type        = string
}

variable "resource_group_name" {
  description = "(Required) Specifies the resource group name"
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

variable "serverless" {
  description = "EnableServerless feature"
  type        = bool
  default     = false
}

variable "databases" {
  type        = map(object({
    scale     = object({
      autoscale      = optional(bool)
      throughput     = optional(number)
      max_throughput = optional(number)
    })
  }))
}