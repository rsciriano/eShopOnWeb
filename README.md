# My eShopOnWeb [WIP]

This is a fork of the eShopOnWeb ASP.NET Core Reference Application, you can read the original README [here](README-MS.md)

## Stock management

```sql
-- Update/Fix Catalog Stock Reserved Quantity
MERGE dbo.CatalogStock as tgt
USING (
	SELECT ItemOrdered_CatalogItemId AS CatalogItemId, sum(Units)
	from dbo.OrderItems 
	group by ItemOrdered_CatalogItemId
) as src (CatalogItemId, Units)
ON tgt.Id = src.CatalogItemId
WHEN MATCHED THEN  
	UPDATE SET ReservedQuantity = src.Units
WHEN NOT MATCHED BY TARGET THEN  
	INSERT (Id, StockQuantity, ReservedQuantity) VALUES (src.CatalogItemId, 0, src.Units);
  
select *
from [dbo].[CatalogStock]

-- Check Stock
select *
from (
	SELECT ItemOrdered_CatalogItemId AS CatalogItemId, sum(Units) as OrderedQuantity
	from dbo.OrderItems 
	group by ItemOrdered_CatalogItemId
) as qOrdered
FULL OUTER JOIN dbo.CatalogStock as qStock ON qStock.Id = qOrdered.CatalogItemId
where qOrdered.CatalogItemId is NULL or qStock.Id is NULL or qOrdered.OrderedQuantity <> qStock.ReservedQuantity
```

## Azure functions (isolated)

- [Guide for running C# Azure Functions in an isolated worker process | Microsoft Learn](https://learn.microsoft.com/en-us/azure/azure-functions/dotnet-isolated-process-guide#dependency-injection)
- [adding AI project by brettsam � Pull Request #944 � Azure/azure-functions-dotnet-worker](https://github.com/Azure/azure-functions-dotnet-worker/pull/944)


## ServiceBus send messages quota exceeded

[Sending Messages at Scale - Cannot allocate more handles | The Long Walk](https://pmichaels.net/2022/09/25/sending-messages-at-scale-cannot-allocate-more-handles/)

[Best practices for improving performance using Azure Service Bus - Azure Service Bus | Microsoft Learn](https://learn.microsoft.com/en-us/azure/service-bus-messaging/service-bus-performance-improvements?tabs=net-standard-sdk-2)