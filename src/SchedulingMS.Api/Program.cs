using Microsoft.EntityFrameworkCore;
using SchedulingMS.Api;
using SchedulingMS.Api.Middleware;
using SchedulingMS.Application;
using SchedulingMS.Infrastructure;
using SchedulingMS.Infrastructure.Persistence;

static bool IsEfDesignTime()
{
    return AppDomain.CurrentDomain.GetAssemblies()
        .Any(assembly => string.Equals(
            assembly.GetName().Name,
            "Microsoft.EntityFrameworkCore.Design",
            StringComparison.OrdinalIgnoreCase));
}

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddApplication()
    .AddInfrastructure(builder.Configuration)
    .AddApi(builder.Configuration);

var app = builder.Build();

if (!IsEfDesignTime())
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<SchedulingDbContext>();
    dbContext.Database.Migrate();
}

app.UseMiddleware<GlobalExceptionMiddleware>();

if (app.Configuration.GetValue("Swagger:Enabled", true))
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "SchedulingMS API v1");
        c.RoutePrefix = "swagger";
    });

    app.MapGet("/", () => Results.Redirect("/swagger"));
}

if (app.Configuration.GetValue("Http:UseHttpsRedirection", false))
{
    app.UseHttpsRedirection();
}

app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();
app.MapGet("/health", () => Results.Ok(new { status = "ok", service = "schedulingms" }));
app.MapControllers();
app.Run();
