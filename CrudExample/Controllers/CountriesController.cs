using Microsoft.AspNetCore.Mvc;
using ServiceContracts;

namespace CrudExample.Controllers;

[Route("[controller]/[action]")]
public class CountriesController : Controller
{
    private readonly ICountriesService _countriesService;

    public CountriesController(ICountriesService countriesService)
    {
        _countriesService = countriesService;
    }

    public IActionResult UploadFromExcel()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> UploadFromExcel(IFormFile? excelFile)
    {
        if (excelFile == null || excelFile.Length == 0)
        {
            ViewBag.ErrorMessage = "Please select an xlsx file";
            return View();
        }

        if (
            !Path.GetExtension(excelFile.FileName)
                .Equals(".xlsx", StringComparison.OrdinalIgnoreCase)
        )
        {
            ViewBag.ErrorMessage = "Unsupported file. 'xlsx' file is expected";
            return View();
        }

        var countriesInserted = await _countriesService.UploadCountriesFromExcelFile(excelFile);
        ViewBag.Message = $"{countriesInserted} countries inserted successfully";
        return View();
    }
}
