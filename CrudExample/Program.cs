using CrudExample.Middleware;
using CrudExample.StartupExtensions;
using Rotativa.AspNetCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Serilog
builder.Host.UseSerilog(
    (context, services, loggerConfiguration) =>
    {
        loggerConfiguration
            .ReadFrom.Configuration(context.Configuration) // Read configuration settings from built-in IConfiguration
            .ReadFrom.Services(services); // Read out current app services and make them available to serilog
    }
);

builder.Services.ConfigureServices(builder.Configuration);

// Build app
var app = builder.Build();

// Environment setting
if (builder.Environment.IsDevelopment())
    app.UseDeveloperExceptionPage();
else
    app.UseExceptionHandlingMiddleware();

app.UseHttpLogging();

// app.Logger.LogDebug("DEBUG MESSAGE");
// app.Logger.LogInformation("INFORMATION MESSAGE");
// app.Logger.LogWarning("WARNING MESSAGE");
// app.Logger.LogError("ERROR MESSAGE");
// app.Logger.LogCritical("CRITICAL MESSAGE");

// Rotativa setup
if (!builder.Environment.IsEnvironment("Test"))
    RotativaConfiguration.Setup("wwwroot");

app.UseStaticFiles();
app.UseRouting();
app.MapControllers();

app.Run();

public partial class Program { }
