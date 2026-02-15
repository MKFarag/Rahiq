#region Usings

using Application.Interfaces;
using Hangfire;
using HangfireBasicAuthenticationFilter;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Serilog;

#endregion

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDependencies(builder.Configuration);

builder.Host.UseSerilog((context, configuration) => configuration.ReadFrom.Configuration(context.Configuration));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseOpenApi();

    app.UseSwaggerUi(options =>
    {
        options.Path = "/swagger";
    });

    app.UseReDoc(options =>
    {
        options.Path = "/redoc";
        options.DocumentPath = "/swagger/v1/swagger.json";
    });
}

app.UseSerilogRequestLogging();

app.UseHttpsRedirection();

#region Hangfire Dashboard

app.MapHangfireDashboard("/jobs", new DashboardOptions
{
    Authorization =
[
new HangfireCustomBasicAuthenticationFilter
         {
             User = app.Configuration.GetValue<string>("Hangfire:Username"),
             Pass = app.Configuration.GetValue<string>("Hangfire:Password")
         }
],
    DashboardTitle = "Rahiq jobs dashboard",
    IsReadOnlyFunc = context => true
});

#endregion

#region Hangfire Recurring Jobs Scheduling

RecurringJob.AddOrUpdate<INotificationService>(
"CanceledOrder",
x => x.SendCanceledOrderListAsync(),
Cron.Daily);

RecurringJob.AddOrUpdate<INotificationService>(
    "PendingOrder",
    x => x.SendPendingOrderListAsync(),
    Cron.Daily);

RecurringJob.AddOrUpdate<INotificationService>(
    "BundleQuantity",
    x => x.SendQuantityWarningAsync(),
    Cron.Daily);

#endregion

app.UseCors();

app.UseAuthentication();

app.UseAuthorization();

app.UseRateLimiter();

app.MapControllers();

app.UseExceptionHandler();

app.MapStaticAssets();

app.MapHealthChecks("health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.Run();
