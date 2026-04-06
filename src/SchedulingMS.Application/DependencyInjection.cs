using Microsoft.Extensions.DependencyInjection;
using SchedulingMS.Application.Interfaces.Services;
using SchedulingMS.Application.Interfaces.UseCases.Absence;
using SchedulingMS.Application.Interfaces.UseCases.Availability;
using SchedulingMS.Application.Interfaces.UseCases.Reservation;
using SchedulingMS.Application.Services;
using SchedulingMS.Application.UseCases.Absence;
using SchedulingMS.Application.UseCases.Availability;
using SchedulingMS.Application.UseCases.Reservation;

namespace SchedulingMS.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ICreateAvailabilityUseCase, CreateAvailabilityUseCase>();
        services.AddScoped<IUpdateAvailabilityUseCase, UpdateAvailabilityUseCase>();
        services.AddScoped<IDeleteAvailabilityUseCase, DeleteAvailabilityUseCase>();
        services.AddScoped<IGetAvailabilityByTechnicianUseCase, GetAvailabilityByTechnicianUseCase>();

        services.AddScoped<ICreateAbsenceUseCase, CreateAbsenceUseCase>();
        services.AddScoped<IUpdateAbsenceUseCase, UpdateAbsenceUseCase>();
        services.AddScoped<IDeleteAbsenceUseCase, DeleteAbsenceUseCase>();
        services.AddScoped<IGetAbsencesByTechnicianUseCase, GetAbsencesByTechnicianUseCase>();

        services.AddScoped<ICreateReservationUseCase, CreateReservationUseCase>();
        services.AddScoped<ICreateReservationWithOrderUseCase, CreateReservationWithOrderUseCase>();
        services.AddScoped<IGetReservationByIdUseCase, GetReservationByIdUseCase>();
        services.AddScoped<IGetReservationsByClientUseCase, GetReservationsByClientUseCase>();
        services.AddScoped<IGetReservationsByTechnicianUseCase, GetReservationsByTechnicianUseCase>();
        services.AddScoped<IGetReservationsOverviewUseCase, GetReservationsOverviewUseCase>();
        services.AddScoped<IApproveReservationUseCase, ApproveReservationUseCase>();
        services.AddScoped<IConfirmReservationUseCase, ConfirmReservationUseCase>();
        services.AddScoped<IUpdateReservationStatusUseCase, UpdateReservationStatusUseCase>();
        services.AddScoped<IReassignReservationUseCase, ReassignReservationUseCase>();
        services.AddScoped<ICloseOverdueFinalizedReservationsUseCase, CloseOverdueFinalizedReservationsUseCase>();

        services.AddScoped<IPreassignmentService, PreassignmentService>();
        services.AddScoped<ISchedulingConsistencyService, SchedulingConsistencyService>();
        return services;
    }
}


