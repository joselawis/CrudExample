using AutoFixture;
using Entities;
using EntityFrameworkCoreMock;
using Microsoft.EntityFrameworkCore;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Services;
using Xunit.Abstractions;

namespace CrudTests;

public class PersonsServiceTest
{
    private readonly ICountriesService _countriesService;
    private readonly IFixture _fixture;
    private readonly IPersonsService _personsService;
    private readonly ITestOutputHelper _testOutputHelper;

    public PersonsServiceTest(ITestOutputHelper testOutputHelper)
    {
        _fixture = new Fixture();

        var countriesInitialData = new List<Country>();
        var personsInitialData = new List<Person>();

        var dbContextMock = new DbContextMock<ApplicationDbContext>(
            new DbContextOptionsBuilder<ApplicationDbContext>().Options
        );

        var dbContext = dbContextMock.Object;
        dbContextMock.CreateDbSetMock(context => context.Countries, countriesInitialData);
        dbContextMock.CreateDbSetMock(context => context.Persons, personsInitialData);

        _countriesService = new CountriesService(dbContext);
        _personsService = new PersonsService(dbContext, _countriesService);

        _testOutputHelper = testOutputHelper;
    }

    #region GetSortedPersons

    // When we sort based on PersonName in DESC, it should return persons list in descending order
    [Fact]
    public async Task GetSortedPersons()
    {
        var allPersons = await AddDummyPersons();
        var sortedAllPersons = allPersons.OrderByDescending(p => p.PersonName).ToList();

        // Print expected
        _testOutputHelper.WriteLine("Expected:");
        allPersons.ForEach(p => _testOutputHelper.WriteLine(p.ToString()));

        var personsSorted = await _personsService.GetSortedPersons(
            allPersons,
            nameof(PersonResponse.PersonName),
            SortOrderOptions.Desc
        );
        // Print actual
        _testOutputHelper.WriteLine("Actual:");
        personsSorted.ForEach(p => _testOutputHelper.WriteLine(p.ToString()));

        Assert.Equal(3, personsSorted.Count);
        Assert.True(sortedAllPersons.Zip(personsSorted, (a, b) => a.Equals(b)).All(match => match));
        Assert.Equal(sortedAllPersons, personsSorted);
    }

    #endregion

    private async Task<List<PersonResponse>> AddDummyPersons()
    {
        var countryAddRequest1 = _fixture.Create<CountryAddRequest>();
        var countryAddRequest2 = _fixture.Create<CountryAddRequest>();
        var countryAddRequest3 = _fixture.Create<CountryAddRequest>();

        var countryAddResponse1 = await _countriesService.AddCountry(countryAddRequest1);
        var countryAddResponse2 = await _countriesService.AddCountry(countryAddRequest2);
        var countryAddResponse3 = await _countriesService.AddCountry(countryAddRequest3);

        var personAddRequest1 = _fixture
            .Build<PersonAddRequest>()
            .With(e => e.PersonName, "Mario")
            .With(e => e.CountryId, countryAddResponse1.CountryId)
            .With(e => e.Email, "email1@example.com")
            .Create();
        var personAddRequest2 = _fixture
            .Build<PersonAddRequest>()
            .With(e => e.PersonName, "Maria")
            .With(e => e.CountryId, countryAddResponse2.CountryId)
            .With(e => e.Email, "email2@example.com")
            .Create();
        var personAddRequest3 = _fixture
            .Build<PersonAddRequest>()
            .With(e => e.CountryId, countryAddResponse3.CountryId)
            .With(e => e.Email, "email3@example.com")
            .Create();

        var personsRequest = new List<PersonAddRequest>
        {
            personAddRequest1,
            personAddRequest2,
            personAddRequest3
        };
        var allPersons = personsRequest
            .Select(async p => await _personsService.AddPerson(p))
            .Select(p => p.Result)
            .ToList();
        return allPersons;
    }

    #region UpdatePerson

    // When we supply null as PersonUpdateRequest, it should throw ArgumentNullException
    [Fact]
    public async Task UpdatePerson_NullPersonUpdateRequest()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(
            async () => await _personsService.UpdatePerson(null)
        );
    }

    // When we supply invalid person id, it should throw ArgumentException
    [Fact]
    public async Task UpdatePerson_InvalidPersonId()
    {
        var personUpdateRequest = _fixture
            .Build<PersonUpdateRequest>()
            .With(e => e.Email, "something@email.com")
            .Create();
        await Assert.ThrowsAsync<ArgumentException>(
            async () => await _personsService.UpdatePerson(personUpdateRequest)
        );
    }

    // When PersonName is null, it should throw ArgumentException
    [Fact]
    public async Task UpdatePerson_NullPersonName()
    {
        var allPersons = await AddDummyPersons();
        var personUpdateRequest = allPersons[0].ToPersonUpdateRequest();
        personUpdateRequest.PersonName = null;
        await Assert.ThrowsAsync<ArgumentException>(
            async () => await _personsService.UpdatePerson(personUpdateRequest)
        );
    }

    // When we supply a valid person update request, it should update the person
    [Fact]
    public async Task UpdatePerson_ValidPersonUpdateRequest()
    {
        var allPersons = await AddDummyPersons();
        var personUpdateRequest = allPersons[0].ToPersonUpdateRequest();
        personUpdateRequest.PersonName = "Juanito";
        var updatedPerson = await _personsService.UpdatePerson(personUpdateRequest);
        Assert.Equal(personUpdateRequest.PersonName, updatedPerson.PersonName);
    }

    #endregion

    #region GetFilteredPersons

    // If the search text is empty and search by is "PersonName", it should return all the persons
    [Fact]
    public async Task GetFilteredPersons_EmptySearchText()
    {
        var allPersons = await AddDummyPersons();

        // Print expected
        _testOutputHelper.WriteLine("Expected:");
        allPersons.ForEach(p => _testOutputHelper.WriteLine(p.ToString()));

        var personsFromFilteredSearch = await _personsService.GetFilteredPersons(
            nameof(PersonResponse.PersonName),
            ""
        );
        // Print actual
        _testOutputHelper.WriteLine("Actual:");
        personsFromFilteredSearch.ForEach(p => _testOutputHelper.WriteLine(p.ToString()));

        Assert.Equal(3, personsFromFilteredSearch.Count);
        Assert.Equal(allPersons, personsFromFilteredSearch);
        allPersons.ForEach(p => Assert.Contains(p, personsFromFilteredSearch));
    }

    // First we will add few persons, then we will search based on person name with some search string. It should return the matching persons.
    [Fact]
    public async Task GetFilteredPersons_SearchByPersonName()
    {
        var allPersons = await AddDummyPersons();

        // Print expected
        _testOutputHelper.WriteLine("Expected:");
        allPersons.ForEach(p => _testOutputHelper.WriteLine(p.ToString()));

        var personsFromFilteredSearch = await _personsService.GetFilteredPersons(
            nameof(PersonResponse.PersonName),
            "ma"
        );
        // Print actual
        _testOutputHelper.WriteLine("Actual:");
        personsFromFilteredSearch.ForEach(p => _testOutputHelper.WriteLine(p.ToString()));

        Assert.Equal(2, personsFromFilteredSearch.Count);
        allPersons
            .FindAll(p => p.PersonName != null && p.PersonName.Contains("ma"))
            .ForEach(p => Assert.Contains(p, personsFromFilteredSearch));
    }

    #endregion

    #region GetPersonByPersonId

    // If we supply null value as PersonId, it should return null as PersonResponse
    [Fact]
    public async void GetPersonByPersonId_NullPersonId()
    {
        Guid? personId = null;

        var personResponse = await _personsService.GetPersonByPersonId(personId);

        Assert.Null(personResponse);
    }

    // If we supply a valid PersonId, it should return a PersonResponse
    [Fact]
    public async Task GetPersonByPersonId_ValidPersonId()
    {
        var personAddRequest = _fixture
            .Build<PersonAddRequest>()
            .With(e => e.Email, "email@example.com")
            .Create();
        var addPersonResponse = await _personsService.AddPerson(personAddRequest);

        var getPersonResponse = await _personsService.GetPersonByPersonId(
            addPersonResponse.PersonId
        );

        Assert.Equal(addPersonResponse, getPersonResponse);
    }

    #endregion

    #region AddPerson

    // When we supply null value as PersonAddRequest, it should throw ArgumentNullException
    [Fact]
    public async Task AddPerson_NullPerson()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(
            async () => await _personsService.AddPerson(null)
        );
    }

    // When we supply null value as PersonName, it should throw ArgumentException
    [Fact]
    public async Task AddPerson_PersonNameIsNull()
    {
        var personAddRequest = _fixture
            .Build<PersonAddRequest>()
            .With(e => e.PersonName, null as string)
            .Create();
        await Assert.ThrowsAsync<ArgumentException>(
            async () => await _personsService.AddPerson(personAddRequest)
        );
    }

    // When we supply proper person details, it should insert the person into the persons list; and it should return an object of PersonResponse, with newly generated PersonId
    [Fact]
    public async Task AddPerson_ProperPersonDetails()
    {
        var personAddRequest = _fixture
            .Build<PersonAddRequest>()
            .With(e => e.Email, "email@example.com")
            .Create();

        var response = await _personsService.AddPerson(personAddRequest);

        var personsList = await _personsService.GetAllPersons();

        Assert.True(response.PersonId != Guid.Empty);
        Assert.Contains(response, personsList);
    }

    #endregion

    #region GetAllPersons

    // The list of persons should be empty by default
    [Fact]
    public async Task GetAllPersons_EmptyList()
    {
        var persons = await _personsService.GetAllPersons();

        Assert.Empty(persons);
    }

    // First, we will add few Persons, then when we call GetAllPersons, it should return the list of Persons
    [Fact]
    public async Task GetAllPersons_AddFewPersons()
    {
        var allPersons = await AddDummyPersons();

        // Print expected
        _testOutputHelper.WriteLine("Expected:");
        allPersons.ForEach(p => _testOutputHelper.WriteLine(p.ToString()));

        var personsFromGetAllPersons = await _personsService.GetAllPersons();

        // Print actual
        _testOutputHelper.WriteLine("Actual:");
        personsFromGetAllPersons.ForEach(p => _testOutputHelper.WriteLine(p.ToString()));

        Assert.Equal(3, personsFromGetAllPersons.Count);
        Assert.Equal(allPersons, personsFromGetAllPersons);
        allPersons.ForEach(p => Assert.Contains(p, personsFromGetAllPersons));
    }

    #endregion

    #region DeletePerson

    // If you supply an invalid PersonId, it should return false
    [Fact]
    public async Task DeletePerson_InvalidPersonId()
    {
        var result = await _personsService.DeletePerson(Guid.NewGuid());

        Assert.False(result);
    }

    // If you supply a null personId, it should throw ArgumentNullException
    [Fact]
    public async Task DeletePerson_PersonIdIsNull()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(
            async () => await _personsService.DeletePerson(null)
        );
    }

    // If you supply a valid PersonId, it should return true
    [Fact]
    public async Task DeletePerson_ValidPersonId()
    {
        var allPersons = await AddDummyPersons();
        var personId = allPersons[0].PersonId;

        var result = await _personsService.DeletePerson(personId);

        Assert.True(result);
    }

    #endregion
}
