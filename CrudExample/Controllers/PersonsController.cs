using CrudExample.Filters;
using CrudExample.Filters.ActionFilters;
using CrudExample.Filters.AuthorizationFilters;
using CrudExample.Filters.ExceptionFilters;
using CrudExample.Filters.ResourceFilters;
using CrudExample.Filters.ResultFilters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Rotativa.AspNetCore;
using Rotativa.AspNetCore.Options;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;

namespace CrudExample.Controllers;

[Route("[controller]/[action]")]
[TypeFilter(
    typeof(ResponseHeaderActionFilter),
    Arguments = new object[] { "X-Controller-Key", "Controller-Value", 3 },
    Order = 3
)]
[TypeFilter(typeof(HandleExceptionFilter))]
[TypeFilter(typeof(PersonsAlwaysRunResultFilter))]
public class PersonsController : Controller
{
    private readonly ICountriesService _countriesService;
    private readonly ILogger<PersonsController> _logger;
    private readonly IPersonsService _personsService;

    public PersonsController(
        IPersonsService personsService,
        ICountriesService countriesService,
        ILogger<PersonsController> logger
    )
    {
        _personsService = personsService;
        _countriesService = countriesService;
        _logger = logger;
    }

    [Route("/")]
    [Route("")]
    [TypeFilter(typeof(PersonsListActionFilter), Order = 4)]
    [TypeFilter(
        typeof(ResponseHeaderActionFilter),
        Arguments = new object[] { "X-Action-Key", "Action-Value", 1 },
        Order = 1
    )]
    [TypeFilter(typeof(PersonsListResultFilter))]
    [SkipFilter]
    public async Task<IActionResult> Index(
        string searchBy,
        string? searchString,
        string sortBy = nameof(PersonResponse.PersonName),
        SortOrderOptions sortOrder = SortOrderOptions.Asc
    )
    {
        _logger.LogInformation("Index action method of PersonsController");
        _logger.LogDebug(
            "searchBy: {SearchBy}, searchString: {SearchString}, sortBy: {SortBy}, sortOrder: {SortOrder}",
            searchBy,
            searchString,
            sortBy,
            sortOrder
        );
        // Searching
        var allPerson = await _personsService.GetFilteredPersons(searchBy, searchString);

        // Sorting
        var sortedPersons = await _personsService.GetSortedPersons(allPerson, sortBy, sortOrder);

        return View(sortedPersons);
    }

    [HttpGet]
    [Route("")]
    public async Task<IActionResult> Create()
    {
        await ProvideCountries();

        return View();
    }

    [HttpPost]
    [Route("")]
    [TypeFilter(typeof(PersonCreateAndEditPostActionFilter))]
    [TypeFilter(typeof(FeatureDisabledResourceFilter), Arguments = new object[] { false })]
    public async Task<IActionResult> Create(PersonAddRequest personRequest)
    {
        await _personsService.AddPerson(personRequest);

        return RedirectToAction("Index", "Persons");
    }

    [HttpGet]
    [Route("{personId:guid}")]
    // [TypeFilter(typeof(TokenResultFilter))]
    [TypeFilter(typeof(PersonCreateAndEditPostActionFilter))]
    public async Task<IActionResult> Edit(Guid personId)
    {
        var personResponse = await _personsService.GetPersonByPersonId(personId);
        if (personResponse == null)
            return RedirectToAction("Index");

        var personUpdateRequest = personResponse.ToPersonUpdateRequest();
        await ProvideCountries();

        return View(personUpdateRequest);
    }

    [HttpPost]
    [Route("{personId:guid}")]
    [TypeFilter(typeof(PersonCreateAndEditPostActionFilter))]
    [TypeFilter(typeof(TokenAuthorizationFilter))]
    public async Task<IActionResult> Edit(PersonUpdateRequest personRequest, Guid personId)
    {
        var personResponse = await _personsService.GetPersonByPersonId(personRequest.PersonId);
        if (personResponse == null)
            return RedirectToAction("Index");

        await _personsService.UpdatePerson(personRequest);

        return RedirectToAction("Index");
    }

    [HttpGet]
    [Route("{personId:guid}")]
    public async Task<IActionResult> Delete(Guid? personId)
    {
        var personResponse = await _personsService.GetPersonByPersonId(personId);
        if (personResponse == null)
            return RedirectToAction("Index");

        return View(personResponse);
    }

    [HttpPost]
    [Route("{personId:guid}")]
    public async Task<IActionResult> Delete(Guid? personId, PersonUpdateRequest personRequest)
    {
        var personResponse = await _personsService.GetPersonByPersonId(personRequest.PersonId);
        if (personResponse == null)
            return RedirectToAction("Index");

        await _personsService.DeletePerson(personResponse.PersonId);

        return RedirectToAction("Index");
    }

    private async Task ProvideCountries()
    {
        var countries = await _countriesService.GetAllCountries();
        ViewBag.Countries = countries.Select(
            country =>
                new SelectListItem
                {
                    Value = country.CountryId.ToString(),
                    Text = country.CountryName
                }
        );
    }

    public async Task<IActionResult> PersonsPdf()
    {
        // Get all persons
        var persons = await _personsService.GetAllPersons();
        return new ViewAsPdf("PersonsPdf", persons, ViewData)
        {
            PageMargins = new Margins
            {
                Top = 20,
                Right = 20,
                Bottom = 20,
                Left = 20
            },
            PageOrientation = Orientation.Landscape
        };
    }

    public async Task<IActionResult> PersonsCsv()
    {
        var memoryStream = await _personsService.GetPersonsCsv();
        return File(memoryStream, "application/octet-stream", "persons.csv");
    }

    public async Task<IActionResult> PersonsExcel()
    {
        var memoryStream = await _personsService.GetPersonsExcel();
        return File(
            memoryStream,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "persons.xlsx"
        );
    }
}
