using Entities;
using ServiceContracts;
using ServiceContracts.DTO;

namespace Services;

public class CountriesService : ICountriesService
{
    private readonly List<Country> _countries = new();

    public CountryResponse AddCountry(CountryAddRequest? countryAddRequest)
    {
        // Validation: countryAddRequest parameter can't be null
        if (countryAddRequest == null) throw new ArgumentNullException(nameof(countryAddRequest));

        // Validation: countryAddRequest.Name parameter can't be null
        if (countryAddRequest.CountryName == null) throw new ArgumentException(nameof(countryAddRequest.CountryName));

        // Validation: Duplicate CountryNames are not allowed
        if (_countries.Any(c => c.CountryName == countryAddRequest.CountryName))
            throw new ArgumentException("Given country name already exists");

        var country = countryAddRequest.ToCountry();
        country.CountryId = Guid.NewGuid();

        _countries.Add(country);

        return country.ToCountryResponse();
    }

    public List<CountryResponse> GetAllCountries()
    {
        throw new NotImplementedException();
    }
}