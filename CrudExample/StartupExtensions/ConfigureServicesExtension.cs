using CrudExample.Filters.ActionFilters;
using Entities;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.EntityFrameworkCore;
using Repositories;
using RepositoryContracts;
using ServiceContracts;
using Services;

namespace CrudExample.StartupExtensions;

public static class ConfigureServicesExtension
{
    public static IServiceCollection ConfigureServices(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.AddHttpLogging(options =>
        {
            options.LoggingFields =
                HttpLoggingFields.RequestProperties
                | HttpLoggingFields.ResponsePropertiesAndHeaders;
        });

        services.AddTransient<ResponseHeaderActionFilter>();

        // Controller
        services.AddControllersWithViews(options =>
        {
            var logger = services
                .BuildServiceProvider()
                .GetRequiredService<ILogger<ResponseHeaderActionFilter>>();
            options.Filters.Add(
                new ResponseHeaderActionFilter(logger)
                {
                    Key = "X-Global-Key",
                    Value = "Global-Value",
                    Order = 2
                }
            );
        });

        // Dependency Injection
        services.AddScoped<ICountriesRepository, CountriesRepository>();
        services.AddScoped<IPersonsRepository, PersonsRepository>();
        services.AddScoped<ICountriesService, CountriesService>();
        services.AddScoped<IPersonsService, PersonsServiceChild>();
        //services.AddScoped<IPersonsService, PersonsService>();

        // Database context
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));
            options.EnableSensitiveDataLogging();
        });

        services.AddTransient<PersonsListActionFilter>();

        return services;
    }
}
