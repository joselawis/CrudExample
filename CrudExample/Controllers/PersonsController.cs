using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Rotativa.AspNetCore;
using Rotativa.AspNetCore.Options;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;

namespace CrudExample.Controllers;

[Route("[controller]/[action]")]
public class PersonsController : Controller
{
    private readonly ICountriesService _countriesService;
    private readonly IPersonsService _personsService;

    public PersonsController(IPersonsService personsService, ICountriesService countriesService)
    {
        _personsService = personsService;
        _countriesService = countriesService;
    }

    [Route("/")]
    [Route("")]
    public async Task<IActionResult> Index(
        string searchBy,
        string? searchString,
        string sortBy = nameof(PersonResponse.PersonName),
        SortOrderOptions sortOrder = SortOrderOptions.Asc
    )
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
        var allPerson = await _personsService.GetFilteredPersons(searchBy, searchString);
        ViewBag.CurrentSearchBy = searchBy;
        ViewBag.CurrentSearchString = searchString!;

        // Sorting
        var sortedPersons = await _personsService.GetSortedPersons(allPerson, sortBy, sortOrder);
        ViewBag.CurrentSortBy = sortBy;
        ViewBag.CurrentSortOrder = sortOrder;

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
    public async Task<IActionResult> Create(PersonAddRequest request)
    {
        if (!ModelState.IsValid)
        {
            await ProvideCountries();

            ViewBag.Errors = ModelState
                .Values.SelectMany(v => v.Errors)
                .SelectMany(e => e.ErrorMessage)
                .ToList();
            return View(request);
        }

        await _personsService.AddPerson(request);

        return RedirectToAction("Index", "Persons");
    }

    [HttpGet]
    [Route("{personId:guid}")]
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
    public async Task<IActionResult> Edit(Guid personId, PersonUpdateRequest personUpdateRequest)
    {
        var personResponse = await _personsService.GetPersonByPersonId(
            personUpdateRequest.PersonId
        );
        if (personResponse == null)
            return RedirectToAction("Index");

        if (!ModelState.IsValid)
        {
            await ProvideCountries();
            ProvideErrors();
            return View(personResponse.ToPersonUpdateRequest());
        }

        await _personsService.UpdatePerson(personUpdateRequest);

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
    public async Task<IActionResult> Delete(Guid? personId, PersonUpdateRequest personUpdateRequest)
    {
        var personResponse = await _personsService.GetPersonByPersonId(
            personUpdateRequest.PersonId
        );
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

    private void ProvideErrors()
    {
        ViewBag.Errors = ModelState
            .Values.SelectMany(v => v.Errors)
            .SelectMany(e => e.ErrorMessage)
            .ToList();
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
