using CrudExample.Filters.ActionFilters;
using Entities;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.EntityFrameworkCore;
using Repositories;
using RepositoryContracts;
using Rotativa.AspNetCore;
using Serilog;
using ServiceContracts;
using Services;

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

builder.Services.AddHttpLogging(options =>
{
    options.LoggingFields =
        HttpLoggingFields.RequestProperties | HttpLoggingFields.ResponsePropertiesAndHeaders;
});

// Controller
builder.Services.AddControllersWithViews(options =>
{
    var logger = builder
        .Services.BuildServiceProvider()
        .GetRequiredService<ILogger<ResponseHeaderActionFilter>>();
    options.Filters.Add(new ResponseHeaderActionFilter(logger, "X-Global-Key", "Global-Value", 2));
});

// Dependency Injection
builder.Services.AddScoped<ICountriesRepository, CountriesRepository>();
builder.Services.AddScoped<IPersonsRepository, PersonsRepository>();
builder.Services.AddScoped<ICountriesService, CountriesService>();
builder.Services.AddScoped<IPersonsService, PersonsService>();

// Database context
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
    options.EnableSensitiveDataLogging();
});

builder.Services.AddTransient<PersonsListActionFilter>();

// Build app
var app = builder.Build();

// Environment setting
if (builder.Environment.IsDevelopment())
    app.UseDeveloperExceptionPage();

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
