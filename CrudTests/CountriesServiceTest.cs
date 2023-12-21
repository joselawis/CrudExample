using AutoFixture;
using Entities;
using EntityFrameworkCoreMock;
using Microsoft.EntityFrameworkCore;
using ServiceContracts;
using ServiceContracts.DTO;
using Services;

namespace CrudTests;

public class CountriesServiceTest
{
    private readonly ICountriesService _countriesService;
    private readonly IFixture _fixture;

    // constructor
    public CountriesServiceTest()
    {
        _fixture = new Fixture();

        var countriesInitialData = new List<Country>();

        var dbContextMock = new DbContextMock<ApplicationDbContext>(
            new DbContextOptionsBuilder<ApplicationDbContext>().Options
        );

        var dbContext = dbContextMock.Object;
        dbContextMock.CreateDbSetMock(temp => temp.Countries, countriesInitialData);

        _countriesService = new CountriesService(dbContext);
    }

    #region AddCountry

    // When CountryAddRequest is null, it should throw ArgumentNullException
    [Fact]
    public async Task AddCountry_NullCountry()
    {
        // Arrange
        CountryAddRequest? request = null;

        // Assert
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
        {
            // Act
            await _countriesService.AddCountry(request);
        });
    }

    // When the CountryName is null, it should throw ArgumentException
    [Fact]
    public async Task AddCountry_CountryNameIsNull()
    {
        // Arrange
        var request = _fixture
            .Build<CountryAddRequest>()
            .With(temp => temp.CountryName, null as string)
            .Create();

        // Assert
        await Assert.ThrowsAsync<ArgumentException>(async () =>
        {
            // Act
            await _countriesService.AddCountry(request);
        });
    }

    // When the CountryName is duplicate, it should throw ArgumentException
    [Fact]
    public async Task AddCountry_DuplicateCountryName()
    {
        // Arrange
        var request1 = _fixture.Build<CountryAddRequest>().Create();
        var request2 = _fixture
            .Build<CountryAddRequest>()
            .With(temp => temp.CountryName, request1.CountryName)
            .Create();

        // Assert
        await Assert.ThrowsAsync<ArgumentException>(async () =>
        {
            // Act
            await _countriesService.AddCountry(request1);
            await _countriesService.AddCountry(request2);
        });
    }

    // When you supply proper country name, it should insert (add) the country to the existing list of countries
    [Fact]
    public async Task AddCountry_ProperCountryDetails()
    {
        // Arrange
        var request = _fixture.Build<CountryAddRequest>().Create();

        // Act
        var response = await _countriesService.AddCountry(request);
        var countriesFromGetAllCountries = await _countriesService.GetAllCountries();

        // Assert
        Assert.True(response.CountryId != Guid.Empty);
        Assert.Contains(response, countriesFromGetAllCountries);
    }

    #endregion


    #region GetAllCountries

    [Fact]
    // The list of countries should be empty by default (before adding any countries)
    public async Task GetAllCountries_EmptyList()
    {
        // Act
        var actualCountryResponseList = await _countriesService.GetAllCountries();

        // Assert
        Assert.Empty(actualCountryResponseList);
    }

    [Fact]
    public async Task GetAllCountries_AddFewCountries()
    {
        // Arrange
        var countryRequestList = new List<CountryAddRequest>
        {
            _fixture.Build<CountryAddRequest>().Create(),
            _fixture.Build<CountryAddRequest>().Create()
        };

        // Act
        var countriesListFromAddCountry = new List<CountryResponse>();

        foreach (var countryRequest in countryRequestList)
            countriesListFromAddCountry.Add(await _countriesService.AddCountry(countryRequest));

        var actualCountryResponseList = await _countriesService.GetAllCountries();

        // read each element from countries_list_from_add_country
        foreach (var expectedCountry in countriesListFromAddCountry)
            Assert.Contains(expectedCountry, actualCountryResponseList);
    }

    #endregion


    #region GetCountryByCountryID

    [Fact]
    //If we supply null as CountryID, it should return null as CountryResponse
    public async Task GetCountryByCountryID_NullCountryID()
    {
        // Arrange
        Guid? countryId = null;

        // Act
        var countryResponseFromGetMethod = await _countriesService.GetCountryByCountryId(countryId);

        // Assert
        Assert.Null(countryResponseFromGetMethod);
    }

    [Fact]
    // If we supply a valid country id, it should return the matching country details as CountryResponse object
    public async Task GetCountryByCountryID_ValidCountryID()
    {
        // Arrange
        var countryAddRequest = _fixture.Build<CountryAddRequest>().Create();

        var countryResponseFromAdd = await _countriesService.AddCountry(countryAddRequest);

        // Act
        var countryResponseFromGet = await _countriesService.GetCountryByCountryId(
            countryResponseFromAdd.CountryId
        );

        // Assert
        Assert.Equal(countryResponseFromAdd, countryResponseFromGet);
    }

    #endregion
}
