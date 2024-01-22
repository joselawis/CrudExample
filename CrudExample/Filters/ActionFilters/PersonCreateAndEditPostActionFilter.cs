using CrudExample.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Rendering;
using ServiceContracts;

namespace CrudExample.Filters.ActionFilters;

public class PersonCreateAndEditPostActionFilter : IAsyncActionFilter
{
    private readonly ICountriesService _countriesService;

    public PersonCreateAndEditPostActionFilter(ICountriesService countriesService)
    {
        _countriesService = countriesService;
    }

    public async Task OnActionExecutionAsync(
        ActionExecutingContext context,
        ActionExecutionDelegate next
    )
    {
        if (context.Controller is PersonsController personsController)
        {
            if (!personsController.ModelState.IsValid)
            {
                await ProvideCountries(personsController);

                personsController.ViewBag.Errors = personsController
                    .ModelState.Values.SelectMany(v => v.Errors)
                    .SelectMany(e => e.ErrorMessage)
                    .ToList();
                var request = context.ActionArguments["personRequest"];
                context.Result = personsController.View(request); // short-circuits subsequent action filters and action methods
            }
            else
            {
                await next();
            }
        }
        else
        {
            await next();
        }
    }

    private async Task ProvideCountries(Controller personsController)
    {
        var countries = await _countriesService.GetAllCountries();
        personsController.ViewBag.Countries = countries.Select(
            country =>
                new SelectListItem
                {
                    Value = country.CountryId.ToString(),
                    Text = country.CountryName
                }
        );
    }
}
