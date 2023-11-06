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
    CountryResponse AddCountry(CountryAddRequest? countryAddRequest);

    /// <summary>
    ///     Returns a list of all countries
    /// </summary>
    /// <returns>All countries from the list as List of CountryResponse</returns>
    List<CountryResponse> GetAllCountries();
}