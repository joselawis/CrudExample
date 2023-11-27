namespace CrudTests;

public class CountriesServiceTest
{
    /*
    private readonly ICountriesService _countriesService = new CountriesService(false);

    #region GetCountryByCountryId

    // If we supply null country Id then it should return null
    [Fact]
    public void GetCountryByCountryId_NullCountryId()
    {
        Guid? countryId = null;

        var response = _countriesService.GetCountryByCountryId(countryId);

        Assert.Null(response);
    }

    // If we supply a valid country Id then it should return the matching country details as CountryResponse object
    [Fact]
    public void GetCountryByCountryId_ValidCountryId()
    {
        var countryAddRequest = new CountryAddRequest
        {
            CountryName = "China"
        };
        var countryFromAdd = _countriesService.AddCountry(countryAddRequest);

        var countryFromGet = _countriesService.GetCountryByCountryId(countryFromAdd.CountryId);

        Assert.Equal(countryFromAdd, countryFromGet);
    }

    #endregion

    #region AddCountry

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
        var countriesFromGetAllCountries = _countriesService.GetAllCountries();

        Assert.True(response.CountryId != Guid.Empty);
        Assert.Equal(request.CountryName, response.CountryName);
        Assert.Contains(response, countriesFromGetAllCountries);
    }

    #endregion

    #region GetAllCountries

    // The list of countries should be empty by default
    [Fact]
    public void GetAllCountries_EmptyList()
    {
        var countries = _countriesService.GetAllCountries();

        Assert.Empty(countries);
    }

    [Fact]
    public void GetAllCountries_AddFewCountries()
    {
        var countryRequestList = new List<CountryAddRequest>
        {
            new() { CountryName = "USA" }, new() { CountryName = "Japan" },
            new() { CountryName = "Spain" }
        };

        var countriesListFromAddCountry = countryRequestList
            .Select(countryRequest => _countriesService.AddCountry(countryRequest)).ToList();

        var response = _countriesService.GetAllCountries();

        Assert.Equal(countriesListFromAddCountry.Count, response.Count);
        foreach (var expectedCountry in countriesListFromAddCountry) Assert.Contains(expectedCountry, response);

        Assert.Equivalent(countriesListFromAddCountry, response);
    }

    #endregion
    */
}