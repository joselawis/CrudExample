using Entities;
using ServiceContracts;
using ServiceContracts.DTO;

namespace Services;

public class CountriesService : ICountriesService
{
    private readonly List<Country> _countries;

    public CountriesService(bool initialize = true)
    {
        _countries = new List<Country>();
        if (initialize)
            _countries.AddRange(new List<Country>
            {
                new() { CountryId = Guid.Parse("055338DF-6A83-4F8B-9FB5-06C87D9A7AEA"), CountryName = "Spain" },
                new() { CountryId = Guid.Parse("8E7E9725-E2F8-460A-B5A1-8027EA8AE966"), CountryName = "France" },
                new() { CountryId = Guid.Parse("2C9DDA3C-F729-40C4-A32E-18D072CE23EA"), CountryName = "Germany" },
                new() { CountryId = Guid.Parse("743CC7A2-EDC1-4757-85C2-6502EE650984"), CountryName = "Sweden" },
                new() { CountryId = Guid.Parse("B24DC622-A001-4FDC-B7CA-C7E96E999C26"), CountryName = "Mexico" }
            });
    }

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
        return _countries.Select(c => c.ToCountryResponse()).ToList();
    }

    public CountryResponse? GetCountryByCountryId(Guid? countryId)
    {
        return countryId == null
            ? null
            : _countries.FirstOrDefault(c => c?.CountryId == countryId, null)?.ToCountryResponse();
    }
}