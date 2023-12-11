using Entities;
using Microsoft.EntityFrameworkCore;
using ServiceContracts;
using ServiceContracts.DTO;
using Services;

namespace CrudTests;

public class CountriesServiceTest
{
    private readonly ICountriesService _countriesService = new CountriesService(
        new PersonsDbContext(new DbContextOptionsBuilder<PersonsDbContext>().Options));

    #region GetCountryByCountryId

    // If we supply null country Id then it should return null
    [Fact]
    public async Task GetCountryByCountryId_NullCountryId()
    {
        Guid? countryId = null;

        var response = await _countriesService.GetCountryByCountryId(countryId);

        Assert.Null(response);
    }

    // If we supply a valid country Id then it should return the matching country details as CountryResponse object
    [Fact]
    public async Task GetCountryByCountryId_ValidCountryId()
    {
        var countryAddRequest = new CountryAddRequest
        {
            CountryName = "China"
        };
        var countryFromAdd = await _countriesService.AddCountry(countryAddRequest);

        var countryFromGet = await _countriesService.GetCountryByCountryId(countryFromAdd.CountryId);

        Assert.Equal(countryFromAdd, countryFromGet);
    }

    #endregion

    #region AddCountry

    // When CountryAddRequest is null, it should throw ArgumentNullException
    [Fact]
    public async Task AddCountry_NullCountry()
    {
        CountryAddRequest? request = null;

        await Assert.ThrowsAsync<ArgumentNullException>(async () => { await _countriesService.AddCountry(request); });
    }

    // When the CountryName is null, it should throw ArgumentException
    [Fact]
    public async Task AddCountry_CountryNameIsNull()
    {
        var request = new CountryAddRequest
        {
            CountryName = null
        };

        await Assert.ThrowsAsync<ArgumentException>(async () => { await _countriesService.AddCountry(request); });
    }

    // When the CountryName is duplicate, it should throw ArgumentException
    [Fact]
    public async Task AddCountry_DuplicatedCountryName()
    {
        var request = new CountryAddRequest
        {
            CountryName = "USA"
        };
        var request2 = new CountryAddRequest
        {
            CountryName = "USA"
        };

        await Assert.ThrowsAsync<ArgumentException>(async () =>
        {
            await _countriesService.AddCountry(request);
            await _countriesService.AddCountry(request2);
        });
    }

    // When you supply proper CountryName, it should insert (add) the country to the existing list of countries
    [Fact]
    public async Task AddCountry_ProperCountryDetails()
    {
        var request = new CountryAddRequest
        {
            CountryName = "Japan"
        };

        var response = await _countriesService.AddCountry(request);
        var countriesFromGetAllCountries = await _countriesService.GetAllCountries();

        Assert.True(response.CountryId != Guid.Empty);
        Assert.Equal(request.CountryName, response.CountryName);
        Assert.Contains(response, countriesFromGetAllCountries);
    }

    #endregion

    #region GetAllCountries

    // The list of countries should be empty by default
    [Fact]
    public async Task GetAllCountries_EmptyList()
    {
        var countries = await _countriesService.GetAllCountries();

        Assert.Empty(countries);
    }

    [Fact]
    public async Task GetAllCountries_AddFewCountries()
    {
        var countryRequestList = new List<CountryAddRequest>
        {
            new() { CountryName = "USA" }, new() { CountryName = "Japan" },
            new() { CountryName = "Spain" }
        };

        var countriesListFromAddCountry = countryRequestList
            .Select(async countryRequest => await _countriesService.AddCountry(countryRequest))
            .Select(c => c.Result).ToList();

        var response = await _countriesService.GetAllCountries();

        Assert.Equal(countriesListFromAddCountry.Count, response.Count);
        foreach (var expectedCountry in countriesListFromAddCountry)
            Assert.Contains(expectedCountry, response);

        Assert.Equivalent(countriesListFromAddCountry, response);
    }

    #endregion
}