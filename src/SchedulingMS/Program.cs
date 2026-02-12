using Microsoft.EntityFrameworkCore;
using SchedulingMS.Application.Interfaces;
using SchedulingMS.Application.UseCases;
using SchedulingMS.Infrastructure.Events;
using SchedulingMS.Infrastructure.Persistence;
using SchedulingMS.Infrastructure.Repositories;
using SchedulingMS.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<SchedulingDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IAvailabilityRepository, SqlAvailabilityRepository>();
builder.Services.AddScoped<IReservationRepository, SqlReservationRepository>();
builder.Services.AddScoped<IAvailabilityService, AvailabilityService>();
builder.Services.AddScoped<IReservationService, ReservationService>();
builder.Services.AddSingleton<IEventPublisher, NoOpEventPublisher>();
builder.Services.AddSingleton<ITechnicianDirectoryGateway, InMemoryTechnicianDirectoryGateway>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<SchedulingDbContext>();
    dbContext.Database.Migrate();
}

app.UseMiddleware<GlobalExceptionMiddleware>();

var swaggerEnabled = app.Configuration.GetValue("Swagger:Enabled", true);
if (swaggerEnabled)
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "SchedulingMS API v1");
        c.RoutePrefix = "swagger";
        c.DocumentTitle = "SchedulingMS - Swagger UI";
    });

    app.MapGet("/", () => Results.Redirect("/swagger"));
}

var useHttpsRedirection = app.Configuration.GetValue("Http:UseHttpsRedirection", false);
if (useHttpsRedirection)
{
    app.UseHttpsRedirection();
}

app.MapControllers();
app.Run();
