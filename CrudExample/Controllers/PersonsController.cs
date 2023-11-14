using Microsoft.AspNetCore.Mvc;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;

namespace CrudExample.Controllers;

public class PersonsController : Controller
{
    private readonly IPersonsService _personsService;

    public PersonsController(IPersonsService personsService)
    {
        _personsService = personsService;
    }

    [Route("persons/index")]
    [Route("/")]
    public IActionResult Index(string searchBy, string? searchString,
        string sortBy = nameof(PersonResponse.PersonName), SortOrderOptions sortOrder = SortOrderOptions.Asc)
    {
        // Searching
        ViewBag.SearchField = new Dictionary<string, string>
        {
            { nameof(PersonResponse.PersonName), "Person Name" },
            { nameof(PersonResponse.Email), "Email" },
            { nameof(PersonResponse.DateOfBirth), "Date Of Birth" },
            { nameof(PersonResponse.Gender), "Gender" },
            { nameof(PersonResponse.CountryId), "Country" },
            { nameof(PersonResponse.Address), "Address" }
        };
        var allPerson = _personsService.GetFilteredPersons(searchBy, searchString);
        ViewBag.CurrentSearchBy = searchBy;
        ViewBag.CurrentSearchString = searchString!;

        // Sorting
        var sortedPersons = _personsService.GetSortedPersons(allPerson, sortBy, sortOrder);
        ViewBag.CurrentSortBy = sortBy;
        ViewBag.CurrentSortOrder = sortOrder;

        return View(sortedPersons);
    }
}