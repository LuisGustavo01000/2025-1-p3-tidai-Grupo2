using Microsoft.AspNetCore.Mvc.Testing;

namespace CashWiseAPI.Tests;

public class StartupTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public StartupTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Testing");
        });
    }

    [Fact]
    public async Task App_Starts_With_InMemory_Configuration()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/swagger/index.html");

        Assert.True(response.IsSuccessStatusCode);
    }
}
