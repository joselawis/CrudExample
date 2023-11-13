using Microsoft.AspNetCore.Mvc;
using ServiceContracts;

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
    public IActionResult Index()
    {
        var allPerson = _personsService.GetAllPersons();
        return View(allPerson);
    }
}