using System.Net;
using System.Text;
using Microsoft.Extensions.Logging.Abstractions;
using SchedulingMS.Infrastructure.Ports;
using Xunit;

namespace SchedulingMS.UnitTests;

public class HttpTechnicianDirectoryGatewayTests
{
    [Fact]
    public async Task GetActiveTechniciansByProviderAsync_FiltersInactiveTechnicians()
    {
        var providerEntityId = Guid.NewGuid();
        var activeTechnicianId = Guid.NewGuid();
        var inactiveTechnicianId = Guid.NewGuid();
        var now = DateTime.UtcNow;

        var payload = $$"""
        [
          {
            "id": "{{activeTechnicianId}}",
            "authUserId": "{{Guid.NewGuid()}}",
            "providerEntityId": "{{providerEntityId}}",
            "specialty": "General",
            "status": 1,
            "createdAtUtc": "{{now:O}}",
            "updatedAtUtc": "{{now:O}}"
          },
          {
            "id": "{{inactiveTechnicianId}}",
            "authUserId": "{{Guid.NewGuid()}}",
            "providerEntityId": "{{providerEntityId}}",
            "specialty": "General",
            "status": 3,
            "createdAtUtc": "{{now:O}}",
            "updatedAtUtc": "{{now:O}}"
          }
        ]
        """;

        var httpClient = new HttpClient(new StubHttpMessageHandler(HttpStatusCode.OK, payload))
        {
            BaseAddress = new Uri("http://localhost:5000/")
        };

        var gateway = new HttpTechnicianDirectoryGateway(httpClient, NullLogger<HttpTechnicianDirectoryGateway>.Instance);

        var result = await gateway.GetActiveTechniciansByProviderAsync(providerEntityId);

        var technician = Assert.Single(result);
        Assert.Equal(activeTechnicianId, technician.TechnicianId);
        Assert.Equal(providerEntityId, technician.ProviderEntityId);
        Assert.True(technician.IsActive);
    }

    [Fact]
    public async Task GetActiveTechniciansByProviderAsync_WhenDirectoryReturnsError_ThrowsInvalidOperationException()
    {
        var providerEntityId = Guid.NewGuid();

        var httpClient = new HttpClient(new StubHttpMessageHandler(HttpStatusCode.BadGateway, "Directory unavailable"))
        {
            BaseAddress = new Uri("http://localhost:5000/")
        };

        var gateway = new HttpTechnicianDirectoryGateway(httpClient, NullLogger<HttpTechnicianDirectoryGateway>.Instance);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            gateway.GetActiveTechniciansByProviderAsync(providerEntityId));
    }

    private sealed class StubHttpMessageHandler(HttpStatusCode statusCode, string content) : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            => Task.FromResult(new HttpResponseMessage(statusCode)
            {
                Content = new StringContent(content, Encoding.UTF8, "application/json")
            });
    }
}
