using Entities;
using Microsoft.AspNetCore.Http;
using OfficeOpenXml;
using RepositoryContracts;
using ServiceContracts;
using ServiceContracts.DTO;

namespace Services;

public class CountriesService : ICountriesService
{
    private readonly ICountriesRepository _countriesRepository;

    public CountriesService(ICountriesRepository countriesRepository)
    {
        _countriesRepository = countriesRepository;
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
        if (
            await _countriesRepository.GetCountryByCountryName(countryAddRequest.CountryName)
            != null
        )
            throw new ArgumentException("Given country name already exists");

        var country = countryAddRequest.ToCountry();
        country.CountryId = Guid.NewGuid();

        await _countriesRepository.AddCountry(country);

        return country.ToCountryResponse();
    }

    public async Task<List<CountryResponse>> GetAllCountries()
    {
        var countries = await _countriesRepository.GetAllCountries();
        return countries.Select(country => country.ToCountryResponse()).ToList();
    }

    public async Task<CountryResponse?> GetCountryByCountryId(Guid? countryId)
    {
        if (countryId == null)
            return null;

        var country = await _countriesRepository.GetCountryByCountryId(countryId.Value);
        return country?.ToCountryResponse();
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
            var countryName = Convert.ToString(workSheet.Cells[row, 1].Value);

            if (string.IsNullOrEmpty(countryName))
                continue;

            if (await _countriesRepository.GetCountryByCountryName(countryName) != null)
                continue;

            var country = new Country { CountryName = countryName };
            await _countriesRepository.AddCountry(country);

            countriesInserted++;
        }

        return countriesInserted;
    }
}
