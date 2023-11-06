using ServiceContracts;
using ServiceContracts.DTO;
using Services;

namespace CrudTests;

public class CountriesServiceTest
{
    private readonly ICountriesService _countriesService = new CountriesService();

    // When CountryAddRequest is null, it should throw ArgumentNullException
    [Fact]
    public void AddCountry_NullCountry()
    {
        CountryAddRequest? request = null;

        Assert.Throws<ArgumentNullException>(() => { _countriesService.AddCountry(request); });
    }

    // When the CountryName is null, it should throw ArgumentException
    [Fact]
    public void AddCountry_CountryNameIsNull()
    {
        var request = new CountryAddRequest
        {
            CountryName = null
        };

        Assert.Throws<ArgumentException>(() => { _countriesService.AddCountry(request); });
    }

    // When the CountryName is duplicate, it should throw ArgumentException
    [Fact]
    public void AddCountry_DuplicatedCountryName()
    {
        var request = new CountryAddRequest
        {
            CountryName = "USA"
        };
        var request2 = new CountryAddRequest
        {
            CountryName = "USA"
        };

        Assert.Throws<ArgumentException>(() =>
        {
            _countriesService.AddCountry(request);
            _countriesService.AddCountry(request2);
        });
    }

    // When you supply proper CountryName, it should insert (add) the country to the existing list of countries
    [Fact]
    public void AddCountry_ProperCountryDetails()
    {
        var request = new CountryAddRequest
        {
            CountryName = "Japan"
        };

        var response = _countriesService.AddCountry(request);

        Assert.True(response.CountryId != Guid.Empty);
        Assert.Equal(request.CountryName, response.CountryName);
    }
}