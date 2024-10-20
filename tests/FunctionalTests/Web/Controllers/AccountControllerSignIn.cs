using System.Net;
using System.Text.RegularExpressions;
using BlazorAdmin;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.eShopWeb.Infrastructure.Data;
using Microsoft.eShopWeb.Infrastructure.Identity;
using Microsoft.Extensions.Hosting;
using Xunit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace Microsoft.eShopWeb.FunctionalTests.Web.Controllers;

[Collection("Database collection")]
public class AccountControllerSignIn : IAsyncLifetime
{
    [Fact]
    public async Task ReturnsSignInScreenOnGet()
    {
        var response = await Client.GetAsync("/identity/account/login");
        response.EnsureSuccessStatusCode();
        var stringResponse = await response.Content.ReadAsStringAsync();

        Assert.Contains("demouser@microsoft.com", stringResponse);
    }

    [Fact]
    public void RegexMatchesValidRequestVerificationToken()
    {
        // TODO: Move to a unit test
        // TODO: Move regex to a constant in test project
        var input = @"<input name=""__RequestVerificationToken"" type=""hidden"" value=""CfDJ8Obhlq65OzlDkoBvsSX0tgxFUkIZ_qDDSt49D_StnYwphIyXO4zxfjopCWsygfOkngsL6P0tPmS2HTB1oYW-p_JzE0_MCFb7tF9Ol_qoOg_IC_yTjBNChF0qRgoZPmKYOIJigg7e2rsBsmMZDTdbnGo"" /><input name=""RememberMe"" type=""hidden"" value=""false"" /></form>";
        string regexpression = @"name=""__RequestVerificationToken"" type=""hidden"" value=""([-A-Za-z0-9+=/\\_]+?)""";
        var regex = new Regex(regexpression);
        var match = regex.Match(input);
        var group = match.Groups.Values.LastOrDefault();
        Assert.NotNull(group);
        Assert.True(group.Value.Length > 50);
    }

    [Fact]
    public async Task ReturnsFormWithRequestVerificationToken()
    {
        var response = await Client.GetAsync("/identity/account/login");
        response.EnsureSuccessStatusCode();
        var stringResponse = await response.Content.ReadAsStringAsync();

        string token = WebPageHelpers.GetRequestVerificationToken(stringResponse);
        Assert.True(token.Length > 50);
    }

    [Fact]
    public async Task ReturnsSuccessfulSignInOnPostWithValidCredentials()
    {
        var getResponse = await Client.GetAsync("/identity/account/login");
        getResponse.EnsureSuccessStatusCode();
        var stringResponse1 = await getResponse.Content.ReadAsStringAsync();

        var keyValues = new List<KeyValuePair<string, string>>
        {
            new KeyValuePair<string, string>("Email", "demouser@microsoft.com"),
            new KeyValuePair<string, string>("Password", "Pass@word1"),
            new KeyValuePair<string, string>(WebPageHelpers.TokenTag, WebPageHelpers.GetRequestVerificationToken(stringResponse1))
        };
        var formContent = new FormUrlEncodedContent(keyValues);

        var postResponse = await Client.PostAsync("/identity/account/login", formContent);
        Assert.Equal(HttpStatusCode.Redirect, postResponse.StatusCode);
        Assert.Equal(new System.Uri("/", UriKind.Relative), postResponse.Headers.Location);
    }

    [Fact]
    public async Task UpdatePhoneNumberProfile()
    {
        //Login
        var getResponse = await Client.GetAsync("/identity/account/login");
        getResponse.EnsureSuccessStatusCode();
        var stringResponse1 = await getResponse.Content.ReadAsStringAsync();
        var keyValues = new List<KeyValuePair<string, string>>
        {
            new KeyValuePair<string, string>("Email", "demouser@microsoft.com"),
            new KeyValuePair<string, string>("Password", "Pass@word1"),
            new KeyValuePair<string, string>(WebPageHelpers.TokenTag, WebPageHelpers.GetRequestVerificationToken(stringResponse1))
        };
        var formContent = new FormUrlEncodedContent(keyValues);
        await Client.PostAsync("/identity/account/login", formContent);

        //Profile page
        var profileResponse = await Client.GetAsync("/manage/my-account");
        profileResponse.EnsureSuccessStatusCode();
        var stringProfileResponse = await profileResponse.Content.ReadAsStringAsync();

        //Update phone number
        var updateProfileValues = new List<KeyValuePair<string, string>>
        {
            new KeyValuePair<string, string>("Email", "demouser@microsoft.com"),
            new KeyValuePair<string, string>("PhoneNumber", "03656565"),
            new KeyValuePair<string, string>(WebPageHelpers.TokenTag, WebPageHelpers.GetRequestVerificationToken(stringProfileResponse))
        };
        var updateProfileContent = new FormUrlEncodedContent(updateProfileValues);
        var postProfileResponse = await Client.PostAsync("/manage/my-account", updateProfileContent);

        Assert.Equal(HttpStatusCode.Redirect, postProfileResponse.StatusCode);
        var profileResponse2 = await Client.GetAsync("/manage/my-account");
        var stringProfileResponse2 = await profileResponse2.Content.ReadAsStringAsync();
        Assert.Contains("03656565", stringProfileResponse2);

    }

    
    private readonly TestApplication _testApplication;
    public HttpClient Client { get; }

    public AccountControllerSignIn(TestApplication testApplication)
    {
        _testApplication = testApplication;

        Client = testApplication.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
    }

    public async Task InitializeAsync()
    {
        var logger = _testApplication.Services.GetRequiredService<ILoggerFactory>().CreateLogger<AccountControllerSignIn>();
        var configuration = _testApplication.Services.GetRequiredService<IConfiguration>();

        using (var scope = _testApplication.Services.CreateScope())
        {
            var scopedProvider = scope.ServiceProvider;
            try
            {
                var catalogContext = scopedProvider.GetRequiredService<CatalogContext>();
                await CatalogContextSeed.SeedAsync(catalogContext, logger);

                var userManager = scopedProvider.GetRequiredService<UserManager<ApplicationUser>>();
                var roleManager = scopedProvider.GetRequiredService<RoleManager<IdentityRole>>();

                IdentityDbContext<ApplicationUser, IdentityRole, string> identityContext = configuration.GetDatabaseEngine() switch
                {
                    DatabaseEngines.CosmosDb => scopedProvider.GetRequiredService<AppIdentityCosmosDbContext>(),
                    _ => scopedProvider.GetRequiredService<AppIdentityDbContext>()
                };
                await AppIdentityDbContextSeed.SeedAsync(identityContext, userManager, roleManager);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred seeding the DB.");
            }
        }

    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }
}
