using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using SchedulingMS.Application.Interfaces.Commands;
using SchedulingMS.Application.Interfaces.Ports;
using SchedulingMS.Application.Interfaces.Queries;
using SchedulingMS.Infrastructure.Commands.Absence;
using SchedulingMS.Infrastructure.Commands.Assignment;
using SchedulingMS.Infrastructure.Commands.Availability;
using SchedulingMS.Infrastructure.Commands.History;
using SchedulingMS.Infrastructure.Commands.Reservations;
using SchedulingMS.Infrastructure.Persistence;
using SchedulingMS.Infrastructure.Ports;
using SchedulingMS.Infrastructure.Queries.Absence;
using SchedulingMS.Infrastructure.Queries.Assignment;
using SchedulingMS.Infrastructure.Queries.Availability;
using SchedulingMS.Infrastructure.Queries.Reservations;
using SchedulingMS.Infrastructure.Workers;

namespace SchedulingMS.Infrastructure;

public static class DependencyInjection
{
    private const string InternalAccessHeaderName = "X-Internal-Key";
    private static readonly MySqlServerVersion MySqlVersion = new(new Version(8, 0, 0));

    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var directoryBaseUrl = configuration["Integrations:DirectoryMS:BaseUrl"];
        if (string.IsNullOrWhiteSpace(directoryBaseUrl))
        {
            throw new InvalidOperationException("Integrations:DirectoryMS:BaseUrl must be configured for SchedulingMS.");
        }

        if (!Uri.TryCreate(directoryBaseUrl, UriKind.Absolute, out var directoryBaseAddress))
        {
            throw new InvalidOperationException("Integrations:DirectoryMS:BaseUrl must be a valid absolute URL.");
        }

        var directoryTimeoutSeconds = configuration.GetValue<int?>("Integrations:DirectoryMS:TimeoutSeconds") ?? 10;
        if (directoryTimeoutSeconds <= 0)
        {
            throw new InvalidOperationException("Integrations:DirectoryMS:TimeoutSeconds must be greater than zero.");
        }

        var orderBaseUrl = configuration["Integrations:OrderMS:BaseUrl"];
        if (string.IsNullOrWhiteSpace(orderBaseUrl))
        {
            throw new InvalidOperationException("Integrations:OrderMS:BaseUrl must be configured for SchedulingMS.");
        }

        if (!Uri.TryCreate(orderBaseUrl, UriKind.Absolute, out var orderBaseAddress))
        {
            throw new InvalidOperationException("Integrations:OrderMS:BaseUrl must be a valid absolute URL.");
        }

        var orderTimeoutSeconds = configuration.GetValue<int?>("Integrations:OrderMS:TimeoutSeconds") ?? 10;
        if (orderTimeoutSeconds <= 0)
        {
            throw new InvalidOperationException("Integrations:OrderMS:TimeoutSeconds must be greater than zero.");
        }

        services.AddDbContext<SchedulingDbContext>(options =>
            options.UseMySql(
                configuration.GetConnectionString("DefaultConnection")
                    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found."),
                MySqlVersion));

        // Commands
        services.AddScoped<IAvailabilityCommand, AvailabilityCommand>();
        services.AddScoped<IAbsenceCommand, AbsenceCommand>();
        services.AddScoped<IReservationCommand, ReservationCommand>();
        services.AddScoped<IAssignmentCommand, AssignmentCommand>();
        services.AddScoped<IReservationStatusHistoryCommand, ReservationStatusHistoryCommand>();

        // Queries
        services.AddScoped<IAvailabilityQuery, AvailabilityQuery>();
        services.AddScoped<IAbsenceQuery, AbsenceQuery>();
        services.AddScoped<IReservationQuery, ReservationQuery>();
        services.AddScoped<IAssignmentQuery, AssignmentQuery>();

        // External ports
        services.AddHttpClient<ITechnicianDirectoryGateway, HttpTechnicianDirectoryGateway>(client =>
        {
            client.BaseAddress = directoryBaseAddress;
            client.Timeout = TimeSpan.FromSeconds(directoryTimeoutSeconds);

            var directoryInternalKey = configuration["Integrations:DirectoryMS:InternalAccessKey"];
            if (!string.IsNullOrWhiteSpace(directoryInternalKey))
            {
                client.DefaultRequestHeaders.Remove(InternalAccessHeaderName);
                client.DefaultRequestHeaders.Add(InternalAccessHeaderName, directoryInternalKey.Trim());
            }
        });
        services.AddHttpClient<IOrderCreationGateway, HttpOrderCreationGateway>(client =>
        {
            client.BaseAddress = orderBaseAddress;
            client.Timeout = TimeSpan.FromSeconds(orderTimeoutSeconds);

            var orderInternalKey = configuration["Integrations:OrderMS:InternalAccessKey"];
            if (!string.IsNullOrWhiteSpace(orderInternalKey))
            {
                client.DefaultRequestHeaders.Remove(InternalAccessHeaderName);
                client.DefaultRequestHeaders.Add(InternalAccessHeaderName, orderInternalKey.Trim());
            }
        });
        services.AddScoped<IUnitOfWork, EfUnitOfWork>();
        services.TryAddScoped<IEventPublisher, StructuredLogEventPublisher>();
        services.AddHostedService<FinalizedReservationAutoCloseWorker>();

        return services;
    }
}


