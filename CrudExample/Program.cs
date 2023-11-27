using Entities;
using Microsoft.EntityFrameworkCore;
using ServiceContracts;
using Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();

builder.Services.AddScoped<ICountriesService, CountriesService>();
builder.Services.AddScoped<IPersonsService, PersonsService>();

builder.Services.AddDbContext<PersonsDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
    options.EnableSensitiveDataLogging();
});

var app = builder.Build();

if (builder.Environment.IsDevelopment()) app.UseDeveloperExceptionPage();

app.UseStaticFiles();
app.UseRouting();
app.MapControllers();

app.Run();