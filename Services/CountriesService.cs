using Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using ServiceContracts;
using ServiceContracts.DTO;

namespace Services;

public class CountriesService : ICountriesService
{
    private readonly ApplicationDbContext _db;

    public CountriesService(ApplicationDbContext applicationDbContext)
    {
        _db = applicationDbContext;
    }

    public async Task<CountryResponse> AddCountry(CountryAddRequest? countryAddRequest)
    {
        // Validation: countryAddRequest parameter can't be null
        if (countryAddRequest == null)
            throw new ArgumentNullException(nameof(countryAddRequest));

        // Validation: countryAddRequest.Name parameter can't be null
        if (countryAddRequest.CountryName == null)
            throw new ArgumentException(nameof(countryAddRequest.CountryName));

        // Validation: Duplicate CountryNames are not allowed
        if (await _db.Countries.AnyAsync(c => c.CountryName == countryAddRequest.CountryName))
            throw new ArgumentException("Given country name already exists");

        var country = countryAddRequest.ToCountry();
        country.CountryId = Guid.NewGuid();

        _db.Countries.Add(country);
        await _db.SaveChangesAsync();

        return country.ToCountryResponse();
    }

    public async Task<List<CountryResponse>> GetAllCountries()
    {
        return await _db.Countries.Select(c => c.ToCountryResponse()).ToListAsync();
    }

    public async Task<CountryResponse?> GetCountryByCountryId(Guid? countryId)
    {
        if (countryId == null)
            return null;

        var countryResponseFromList = await _db.Countries.FirstOrDefaultAsync(
            temp => temp.CountryId == countryId
        );

        return countryResponseFromList?.ToCountryResponse();
    }

    public async Task<int> UploadCountriesFromExcelFile(IFormFile formFile)
    {
        var countriesInserted = 0;

        var memoryStream = new MemoryStream();
        await formFile.CopyToAsync(memoryStream);

        using var excelPackage = new ExcelPackage(memoryStream);
        var workSheet = excelPackage.Workbook.Worksheets["Countries"];

        var rowCount = workSheet.Dimension.Rows;

        for (var row = 2; row <= rowCount; row++)
        {
            var cellValue = Convert.ToString(workSheet.Cells[row, 1].Value);

            if (string.IsNullOrEmpty(cellValue))
                continue;

            var countryName = cellValue;

            if (_db.Countries.Any(c => c.CountryName == countryName))
                continue;

            var country = new Country { CountryName = countryName };
            _db.Countries.Add(country);
            await _db.SaveChangesAsync();

            countriesInserted++;
        }

        return countriesInserted;
    }
}
