using Microsoft.Extensions.Logging;
using SchedulingMS.Application.Interfaces.Ports;

namespace SchedulingMS.Infrastructure.Ports;

public sealed class StructuredLogEventPublisher(ILogger<StructuredLogEventPublisher> logger) : IEventPublisher
{
    public Task PublishAsync<TEvent>(TEvent eventData, CancellationToken cancellationToken = default)
    {
        logger.LogInformation(
            "Integration event recorded for HTTP phase {EventType}: {@EventData}",
            typeof(TEvent).Name,
            eventData);

        return Task.CompletedTask;
    }
}
