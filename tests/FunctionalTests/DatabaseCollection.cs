using Microsoft.eShopWeb.FunctionalTests.Web;
using Xunit;

namespace Microsoft.eShopWeb.FunctionalTests;
[CollectionDefinition("Database collection", DisableParallelization = true)]
public class DatabaseCollection : ICollectionFixture<TestApplication>
{
    // This class has no code, and is never created. Its purpose is simply
    // to be the place to apply [CollectionDefinition] and all the
    // ICollectionFixture<> interfaces.
}
