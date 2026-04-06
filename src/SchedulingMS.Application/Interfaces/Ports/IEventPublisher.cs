namespace SchedulingMS.Application.Interfaces.Ports;

public interface IEventPublisher
{
    Task PublishAsync<TEvent>(TEvent eventData, CancellationToken cancellationToken = default);
}


