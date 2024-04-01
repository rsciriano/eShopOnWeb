using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.eShopWeb.Infrastructure.Data;
public static class DatabaseEngines
{
    public const string InMemory = nameof(InMemory);
    public const string CosmosDb = nameof(CosmosDb);
    public const string SqlServer = nameof(SqlServer);
}
