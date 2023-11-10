using Microsoft.AspNetCore.Mvc;

namespace CrudExample.Controllers;

public class PersonsController : Controller
{
    [Route("persons/index")]
    [Route("/")]
    public IActionResult Index()
    {
        return View();
    }
}