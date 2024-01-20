using AutoFixture;
using Entities;
using FluentAssertions;
using Moq;
using RepositoryContracts;
using ServiceContracts;
using ServiceContracts.DTO;
using Services;

namespace CrudTests;

public class CountriesServiceTest
{
    private readonly Mock<ICountriesRepository> _countriesRepositoryMock;
    private readonly ICountriesService _countriesService;
    private readonly IFixture _fixture;

    // constructor
    public CountriesServiceTest()
    {
        _fixture = new Fixture();

        _countriesRepositoryMock = new Mock<ICountriesRepository>();
        var countriesRepository = _countriesRepositoryMock.Object;

        _countriesService = new CountriesService(countriesRepository);
    }

    #region AddCountry

    // When CountryAddRequest is null, it should throw ArgumentNullException
    [Fact]
    public async Task AddCountry_NullCountry()
    {
        // Arrange
        CountryAddRequest? request = null;

        // Assert
        var action = async () => await _countriesService.AddCountry(request);
        await action.Should().ThrowAsync<ArgumentNullException>();
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
        var action = async () => await _countriesService.AddCountry(request);
        await action.Should().ThrowAsync<ArgumentException>();
    }

    // When the CountryName is duplicate, it should throw ArgumentException
    [Fact]
    public async Task AddCountry_DuplicateCountryName_ToBeArgumentException()
    {
        // Arrange
        var countryAddRequest = _fixture.Build<CountryAddRequest>().Create();

        var country = countryAddRequest.ToCountry();

        _countriesRepositoryMock
            .Setup(temp => temp.GetCountryByCountryName(It.IsAny<string>()))
            .ReturnsAsync(country);

        // Assert
        var action = async () => await _countriesService.AddCountry(countryAddRequest);
        await action.Should().ThrowAsync<ArgumentException>();
    }

    // When you supply proper country name, it should insert (add) the country to the existing list of countries
    [Fact]
    public async Task AddCountry_ProperCountryDetails_ToBeSuccessful()
    {
        // Arrange
        var countryAddRequest = _fixture.Build<CountryAddRequest>().Create();

        var country = countryAddRequest.ToCountry();
        var countryExpected = country.ToCountryResponse();

        _countriesRepositoryMock
            .Setup(temp => temp.GetCountryByCountryName(It.IsAny<string>()))
            .ReturnsAsync(null as Country);

        _countriesRepositoryMock
            .Setup(temp => temp.AddCountry(It.IsAny<Country>()))
            .ReturnsAsync(country);

        // Act
        var response = await _countriesService.AddCountry(countryAddRequest);
        countryExpected.CountryId = response.CountryId;

        // Assert
        response.CountryId.Should().NotBe(Guid.Empty);
        response.Should().Be(countryExpected);
    }

    #endregion


    #region GetAllCountries

    [Fact]
    // The list of countries should be empty by default (before adding any countries)
    public async Task GetAllCountries_EmptyList_ToBeSuccessful()
    {
        var countries = new List<Country>();
        _countriesRepositoryMock.Setup(temp => temp.GetAllCountries()).ReturnsAsync(countries);

        // Act
        var actualCountryResponseList = await _countriesService.GetAllCountries();

        // Assert
        actualCountryResponseList.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAllCountries_AddFewCountries_ToBeSuccessful()
    {
        // Arrange
        var countries = new List<Country>
        {
            _fixture.Build<Country>().With(c => c.Persons, null as List<Person>).Create(),
            _fixture.Build<Country>().With(c => c.Persons, null as List<Person>).Create()
        };

        var countryListExpected = countries.Select(temp => temp.ToCountryResponse()).ToList();

        _countriesRepositoryMock.Setup(temp => temp.GetAllCountries()).ReturnsAsync(countries);

        // Act
        var actualCountryResponseList = await _countriesService.GetAllCountries();

        // assert
        actualCountryResponseList.Should().BeEquivalentTo(countryListExpected);
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
        countryResponseFromGetMethod.Should().BeNull();
    }

    [Fact]
    // If we supply a valid country id, it should return the matching country details as CountryResponse object
    public async Task GetCountryByCountryID_ValidCountryID()
    {
        // Arrange
        var country = _fixture.Build<Country>().With(c => c.Persons, null as List<Person>).Create();

        _countriesRepositoryMock
            .Setup(temp => temp.GetCountryByCountryId(It.IsAny<Guid>()))
            .ReturnsAsync(country);

        var countryExpected = country.ToCountryResponse();

        // Act
        var countryResponseFromGet = await _countriesService.GetCountryByCountryId(
            country.CountryId
        );

        // Assert
        countryResponseFromGet.Should().Be(countryExpected);
    }

    #endregion
}
