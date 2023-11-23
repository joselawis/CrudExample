using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;

namespace CrudExample.Controllers;

[Route("[controller]/[action]/")]
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

    [HttpGet]
    public IActionResult Create()
    {
        ProvideCountries();

        return View();
    }


    [HttpPost]
    public IActionResult Create(PersonAddRequest request)
    {
        if (!ModelState.IsValid)
        {
            ProvideCountries();

            ViewBag.Errors = ModelState.Values.SelectMany(v => v.Errors).SelectMany(e => e.ErrorMessage).ToList();
            return View();
        }

        _personsService.AddPerson(request);

        return RedirectToAction("Index", "Persons");
    }

    [HttpGet]
    [Route("{personId:guid}")]
    public IActionResult Edit(Guid personId)
    {
        var personResponse = _personsService.GetPersonByPersonId(personId);
        if (personResponse == null) return RedirectToAction("Index");

        var personUpdateRequest = personResponse.ToPersonUpdateRequest();
        ProvideCountries();

        return View(personUpdateRequest);
    }

    [HttpPost]
    [Route("{personId:guid}")]
    public IActionResult Edit(Guid personId, PersonUpdateRequest personUpdateRequest)
    {
        var personResponse = _personsService.GetPersonByPersonId(personUpdateRequest.PersonId);
        if (personResponse == null) return RedirectToAction("Index");

        if (!ModelState.IsValid)
        {
            ProvideCountries();
            ProvideErrors();
            return View(personResponse.ToPersonUpdateRequest());
        }

        _personsService.UpdatePerson(personUpdateRequest);

        return RedirectToAction("Index");
    }

    [HttpGet]
    [Route("{personId:guid}")]
    public IActionResult Delete(Guid? personId)
    {
        var personResponse = _personsService.GetPersonByPersonId(personId);
        if (personResponse == null) return RedirectToAction("Index");

        return View(personResponse);
    }

    [HttpPost]
    [Route("{personId:guid}")]
    public IActionResult Delete(Guid? personId, PersonUpdateRequest personUpdateRequest)
    {
        var personResponse = _personsService.GetPersonByPersonId(personUpdateRequest.PersonId);
        if (personResponse == null) return RedirectToAction("Index");

        _personsService.DeletePerson(personResponse.PersonId);

        return RedirectToAction("Index");
    }

    private void ProvideCountries()
    {
        var countries = _countriesService.GetAllCountries();
        ViewBag.Countries = countries.Select(country => new SelectListItem
        {
            Value = country.CountryId.ToString(),
            Text = country.CountryName
        });
    }

    private void ProvideErrors()
    {
        ViewBag.Errors = ModelState.Values.SelectMany(v => v.Errors).SelectMany(e => e.ErrorMessage).ToList();
    }
}