using SchedulingMS.Application.Interfaces;
using SchedulingMS.Application.UseCases;
using SchedulingMS.Infrastructure.Events;
using SchedulingMS.Infrastructure.Repositories;
using SchedulingMS.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IAvailabilityRepository, InMemoryAvailabilityRepository>();
builder.Services.AddScoped<IReservationRepository, InMemoryReservationRepository>();
builder.Services.AddScoped<IAvailabilityService, AvailabilityService>();
builder.Services.AddScoped<IReservationService, ReservationService>();
builder.Services.AddSingleton<IEventPublisher, NoOpEventPublisher>();
builder.Services.AddSingleton<ITechnicianDirectoryGateway, InMemoryTechnicianDirectoryGateway>();

var app = builder.Build();

app.UseMiddleware<GlobalExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();
app.Run();
