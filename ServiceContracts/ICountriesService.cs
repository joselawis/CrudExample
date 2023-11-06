using ServiceContracts.DTO;

namespace ServiceContracts;

/// <summary>
///     Represents business logic for manipulation Country entity
/// </summary>
public interface ICountriesService
{
    CountryResponse AddCountry(CountryAddRequest? request);
}