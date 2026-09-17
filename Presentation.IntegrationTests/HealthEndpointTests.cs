using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;

namespace Presentation.IntegrationTests
{
    public sealed class HealthEndpointTests(WebApplicationFactory<Program> factory) : IClassFixture<WebApplicationFactory<Program>>
    {
        [Fact]
        public async Task GetHealthReturnsOk()
        {
            var client = factory.CreateClient();

            var response = await client.GetAsync(new Uri("/health", UriKind.Relative));

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
