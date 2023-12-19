using Microsoft.AspNetCore.Http;
using ServiceContracts.DTO;

namespace ServiceContracts;

/// <summary>
///     Represents business logic for manipulation Country entity
/// </summary>
public interface ICountriesService
{
    /// <summary>
    ///     Adds a country object to the list of countries
    /// </summary>
    /// <param name="countryAddRequest">Country object to add</param>
    /// <returns>Returns the country object after adding it (including newly generated country id)</returns>
    Task<CountryResponse> AddCountry(CountryAddRequest? countryAddRequest);

    /// <summary>
    ///     Returns a list of all countries
    /// </summary>
    /// <returns>All countries from the list as List of CountryResponse</returns>
    Task<List<CountryResponse>> GetAllCountries();

    /// <summary>
    ///     Returns a country by id
    /// </summary>
    /// <param name="countryId">Country Id (GUID) to search</param>
    /// <returns>Matching country as CountryResponse object</returns>
    Task<CountryResponse?> GetCountryByCountryId(Guid? countryId);

    /// <summary>
    ///     Uploads countries from excel file into database
    /// </summary>
    /// <param name="formFile">Excel file with list of countries</param>
    /// <returns>Returns number of countries added</returns>
    Task<int> UploadCountriesFromExcelFile(IFormFile formFile);
}
