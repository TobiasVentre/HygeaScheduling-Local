using Microsoft.Extensions.Logging;
using SchedulingMS.Application.Interfaces;

namespace SchedulingMS.Infrastructure.Events;

public class NoOpEventPublisher(ILogger<NoOpEventPublisher> logger) : IEventPublisher
{
    public Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default) where TEvent : class
    {
        logger.LogInformation("Publishing integration event {EventType}: {@Event}", typeof(TEvent).Name, @event);
        return Task.CompletedTask;
    }
}
