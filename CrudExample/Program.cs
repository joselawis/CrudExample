using Entities;
using Microsoft.EntityFrameworkCore;
using Repositories;
using RepositoryContracts;
using Rotativa.AspNetCore;
using ServiceContracts;
using Services;

var builder = WebApplication.CreateBuilder(args);

// Logging
builder.Logging.ClearProviders().AddConsole();

// Controller
builder.Services.AddControllersWithViews();

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

// Build app
var app = builder.Build();

// Environment setting
if (builder.Environment.IsDevelopment())
    app.UseDeveloperExceptionPage();

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
