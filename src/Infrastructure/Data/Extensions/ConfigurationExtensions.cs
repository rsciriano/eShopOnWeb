namespace Microsoft.Extensions.Configuration;
public static class ConfigurationExtensions
{
    public static string? GetDatabaseEngine(this IConfiguration configuration)
    {
        return configuration["DatabaseEngine"];
    }
}
