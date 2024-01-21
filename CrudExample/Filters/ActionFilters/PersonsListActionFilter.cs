using CrudExample.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using ServiceContracts.DTO;

namespace CrudExample.Filters.ActionFilters;

public class PersonsListActionFilter : IActionFilter
{
    private readonly ILogger<PersonsListActionFilter> _logger;

    public PersonsListActionFilter(ILogger<PersonsListActionFilter> logger)
    {
        _logger = logger;
    }

    public void OnActionExecuting(ActionExecutingContext context)
    {
        _logger.LogInformation("PersonsListActionFilter.OnActionExecuting method called");
        context.HttpContext.Items.Add("arguments", context.ActionArguments);

        if (!context.ActionArguments.TryGetValue("searchBy", out var argument))
            return;
        var searchBy = Convert.ToString(argument);

        // Validates the searchBy parameter value
        if (string.IsNullOrEmpty(searchBy))
            return;
        var searchByOptions = new List<string>
        {
            nameof(PersonResponse.PersonName),
            nameof(PersonResponse.Email),
            nameof(PersonResponse.DateOfBirth),
            nameof(PersonResponse.Gender),
            nameof(PersonResponse.CountryId),
            nameof(PersonResponse.Address)
        };
        // Reset the searchBy value
        if (searchByOptions.Any(temp => temp == searchBy))
            return;
        _logger.LogInformation("SearchBy actual value {SearchBy}", searchBy);
        context.ActionArguments["searchBy"] = nameof(PersonResponse.PersonName);
        _logger.LogInformation(
            "SearchBy updated value {SearchBy}",
            Convert.ToString(context.ActionArguments["searchBy"])
        );
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        _logger.LogInformation("PersonsListActionFilter.OnActionExecuted method called");
        var personsController = (PersonsController)context.Controller;

        if (context.HttpContext.Items["arguments"] is Dictionary<string, object?> arguments)
        {
            if (arguments.TryGetValue("searchBy", out var searchBy))
                personsController.ViewData["CurrentSearchBy"] = Convert.ToString(searchBy);
            if (arguments.TryGetValue("searchString", out var searchString))
                personsController.ViewData["CurrentSearchString"] = Convert.ToString(searchString);
            if (arguments.TryGetValue("sortBy", out var sortBy))
                personsController.ViewData["CurrentSortBy"] = Convert.ToString(sortBy);
            if (arguments.TryGetValue("sortOrder", out var sortOrder))
                personsController.ViewData["CurrentSortOrder"] = Convert.ToString(sortOrder);
        }

        personsController.ViewBag.SearchField = new Dictionary<string, string>
        {
            { nameof(PersonResponse.PersonName), "Person Name" },
            { nameof(PersonResponse.Email), "Email" },
            { nameof(PersonResponse.DateOfBirth), "Date Of Birth" },
            { nameof(PersonResponse.Gender), "Gender" },
            { nameof(PersonResponse.CountryId), "Country" },
            { nameof(PersonResponse.Address), "Address" }
        };
    }
}
