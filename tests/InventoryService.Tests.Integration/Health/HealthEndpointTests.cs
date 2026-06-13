using InventoryService.Tests.Integration.Support;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace InventoryService.Tests.Integration.Health;

public sealed class HealthEndpointTests(
    InventoryServiceWebApplicationFactory factory)
    : IClassFixture<InventoryServiceWebApplicationFactory>
{
    [Fact]
    public async Task GetHealth_WhenApiAndDatabaseAreAvailable_ReturnsHealthy()
    {
        using var client = factory.CreateClient();

        using var response = await client.GetAsync("/health");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync();
        using var jsonDocument = JsonDocument.Parse(content);

        var status = jsonDocument.RootElement
            .GetProperty("status")
            .GetString();

        Assert.Equal("Healthy", status);
    }
}