#region Usings

using Hangfire;
using HangfireBasicAuthenticationFilter;
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

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.UseExceptionHandler();

app.Run();
