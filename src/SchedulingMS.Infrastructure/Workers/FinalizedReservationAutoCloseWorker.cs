using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SchedulingMS.Application.Interfaces.UseCases.Reservation;

namespace SchedulingMS.Infrastructure.Workers;

public class FinalizedReservationAutoCloseWorker(
    IServiceScopeFactory serviceScopeFactory,
    IConfiguration configuration,
    ILogger<FinalizedReservationAutoCloseWorker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!configuration.GetValue("Automation:AutoClose:Enabled", true))
        {
            logger.LogInformation("Automatic reservation closure is disabled by configuration.");
            return;
        }

        var intervalSeconds = configuration.GetValue("Automation:AutoClose:PollIntervalSeconds", 300);
        using var timer = new PeriodicTimer(TimeSpan.FromSeconds(Math.Max(intervalSeconds, 30)));

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = serviceScopeFactory.CreateScope();
                var useCase = scope.ServiceProvider.GetRequiredService<ICloseOverdueFinalizedReservationsUseCase>();
                await useCase.ExecuteAsync(stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error while auto-closing finalized reservations.");
            }

            await timer.WaitForNextTickAsync(stoppingToken);
        }
    }
}
