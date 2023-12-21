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

    // private fields
    private readonly IPersonsService _personsService;
    private readonly ITestOutputHelper _testOutputHelper;

    // constructor
    public PersonsServiceTest(ITestOutputHelper testOutputHelper)
    {
        _fixture = new Fixture();

        var countriesInitialData = new List<Country>();
        var personsInitialData = new List<Person>();

        // Create mock for DbContext
        var dbContextMock = new DbContextMock<ApplicationDbContext>(
            new DbContextOptionsBuilder<ApplicationDbContext>().Options
        );

        // Access Mock DbContext object
        var dbContext = dbContextMock.Object;

        // Create mocks for DbSets'
        dbContextMock.CreateDbSetMock(temp => temp.Countries, countriesInitialData);
        dbContextMock.CreateDbSetMock(temp => temp.Persons, personsInitialData);

        // Create services based on mocked DbContext object
        _countriesService = new CountriesService(dbContext);
        _personsService = new PersonsService(dbContext, _countriesService);

        _testOutputHelper = testOutputHelper;
    }

    #region GetSortedPersons

    // When we sort based on PersonName in DESC, it should return persons list in descending order
    [Fact]
    public async Task GetSortedPersons()
    {
        // Arrange
        var countryRequest1 = _fixture.Create<CountryAddRequest>();
        var countryRequest2 = _fixture.Create<CountryAddRequest>();

        var countryResponse1 = await _countriesService.AddCountry(countryRequest1);
        var countryResponse2 = await _countriesService.AddCountry(countryRequest2);

        var personRequest1 = _fixture
            .Build<PersonAddRequest>()
            .With(temp => temp.PersonName, "Smith")
            .With(temp => temp.Email, "someone_1@example.com")
            .With(temp => temp.CountryId, countryResponse1.CountryId)
            .Create();

        var personRequest2 = _fixture
            .Build<PersonAddRequest>()
            .With(temp => temp.PersonName, "Mary")
            .With(temp => temp.Email, "someone_2@example.com")
            .With(temp => temp.CountryId, countryResponse1.CountryId)
            .Create();

        var personRequest3 = _fixture
            .Build<PersonAddRequest>()
            .With(temp => temp.PersonName, "Rahman")
            .With(temp => temp.Email, "someone_3@example.com")
            .With(temp => temp.CountryId, countryResponse2.CountryId)
            .Create();

        var personRequests = new List<PersonAddRequest>
        {
            personRequest1,
            personRequest2,
            personRequest3
        };

        var personResponseListFromAdd = new List<PersonResponse>();
        personRequests.ForEach(personRequest =>
        {
            var personResponse = _personsService.AddPerson(personRequest).Result;
            personResponseListFromAdd.Add(personResponse);
        });

        // print person_response_list_from_add
        _testOutputHelper.WriteLine("Expected:");
        foreach (var personResponseFromAdd in personResponseListFromAdd)
            _testOutputHelper.WriteLine(personResponseFromAdd.ToString());
        var allPersons = await _personsService.GetAllPersons();

        // Act
        var personsListFromSort = await _personsService.GetSortedPersons(
            allPersons,
            nameof(Person.PersonName),
            SortOrderOptions.Desc
        );

        // print persons_list_from_get
        _testOutputHelper.WriteLine("Actual:");
        foreach (var personResponseFromGet in personsListFromSort)
            _testOutputHelper.WriteLine(personResponseFromGet.ToString());
        personResponseListFromAdd = personResponseListFromAdd
            .OrderByDescending(temp => temp.PersonName)
            .ToList();

        // Assert
        for (var i = 0; i < personResponseListFromAdd.Count; i++)
            Assert.Equal(personResponseListFromAdd[i], personsListFromSort[i]);
    }

    #endregion

    #region AddPerson

    // When we supply null value as PersonAddRequest, it should throw ArgumentNullException
    [Fact]
    public async Task AddPerson_NullPerson()
    {
        // Arrange
        PersonAddRequest? personAddRequest = null;

        // Act
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
        {
            await _personsService.AddPerson(personAddRequest);
        });
    }

    //When we supply null value as PersonName, it should throw ArgumentException
    [Fact]
    public async Task AddPerson_PersonNameIsNull()
    {
        // Arrange
        var personAddRequest = _fixture
            .Build<PersonAddRequest>()
            .With(temp => temp.PersonName, null as string)
            .Create();

        // Assert
        await Assert.ThrowsAsync<ArgumentException>(async () =>
        {
            // Act
            await _personsService.AddPerson(personAddRequest);
        });
    }

    //When we supply proper person details, it should insert the person into the persons list; and it should return an object of PersonResponse, which includes with the newly generated person id
    [Fact]
    public async Task AddPerson_ProperPersonDetails()
    {
        // Arrange
        var personAddRequest = _fixture
            .Build<PersonAddRequest>()
            .With(temp => temp.Email, "someone@example.com")
            .Create();

        // Act
        var personResponseFromAdd = await _personsService.AddPerson(personAddRequest);

        var personsList = await _personsService.GetAllPersons();

        // Assert
        Assert.True(personResponseFromAdd.PersonId != Guid.Empty);

        Assert.Contains(personResponseFromAdd, personsList);
    }

    #endregion


    #region GetPersonByPersonID

    // If we supply null as PersonID, it should return null as PersonResponse
    [Fact]
    public async Task GetPersonByPersonID_NullPersonID()
    {
        // Arrange
        Guid? personId = null;

        // Act
        var personResponseFromGet = await _personsService.GetPersonByPersonId(personId);

        // Assert
        Assert.Null(personResponseFromGet);
    }

    // If we supply a valid person id, it should return the valid person details as PersonResponse object
    [Fact]
    public async Task GetPersonByPersonID_WithPersonID()
    {
        // Arrange
        var personRequest = _fixture
            .Build<PersonAddRequest>()
            .With(temp => temp.Email, "email@sample.com")
            .Create();

        var personResponseFromAdd = await _personsService.AddPerson(personRequest);

        var personResponseFromGet = await _personsService.GetPersonByPersonId(
            personResponseFromAdd.PersonId
        );

        // Assert
        Assert.Equal(personResponseFromAdd, personResponseFromGet);
    }

    #endregion


    #region GetAllPersons

    // The GetAllPersons() should return an empty list by default
    [Fact]
    public async Task GetAllPersons_EmptyList()
    {
        // Act
        var personsFromGet = await _personsService.GetAllPersons();

        // Assert
        Assert.Empty(personsFromGet);
    }

    // First, we will add few persons; and then when we call GetAllPersons(), it should return the same persons that were added
    [Fact]
    public async Task GetAllPersons_AddFewPersons()
    {
        // Arrange
        var personRequest1 = _fixture
            .Build<PersonAddRequest>()
            .With(temp => temp.Email, "someone_1@example.com")
            .Create();

        var personRequest2 = _fixture
            .Build<PersonAddRequest>()
            .With(temp => temp.Email, "someone_2@example.com")
            .Create();

        var personRequest3 = _fixture
            .Build<PersonAddRequest>()
            .With(temp => temp.Email, "someone_3@example.com")
            .Create();

        var personRequests = new List<PersonAddRequest>
        {
            personRequest1,
            personRequest2,
            personRequest3
        };

        var personResponseListFromAdd = new List<PersonResponse>();

        foreach (var personRequest in personRequests)
        {
            var personResponse = await _personsService.AddPerson(personRequest);
            personResponseListFromAdd.Add(personResponse);
        }

        // print person_response_list_from_add
        _testOutputHelper.WriteLine("Expected:");
        foreach (var personResponseFromAdd in personResponseListFromAdd)
            _testOutputHelper.WriteLine(personResponseFromAdd.ToString());

        // Act
        var personsListFromGet = await _personsService.GetAllPersons();

        // print persons_list_from_get
        _testOutputHelper.WriteLine("Actual:");
        foreach (var personResponseFromGet in personsListFromGet)
            _testOutputHelper.WriteLine(personResponseFromGet.ToString());

        // Assert
        foreach (var personResponseFromAdd in personResponseListFromAdd)
            Assert.Contains(personResponseFromAdd, personsListFromGet);
    }

    #endregion


    #region GetFilteredPersons

    // If the search text is empty and search by is "PersonName", it should return all persons
    [Fact]
    public async Task GetFilteredPersons_EmptySearchText()
    {
        var personRequest1 = _fixture
            .Build<PersonAddRequest>()
            .With(temp => temp.Email, "someone_1@example.com")
            .Create();

        var personRequest2 = _fixture
            .Build<PersonAddRequest>()
            .With(temp => temp.Email, "someone_2@example.com")
            .Create();

        var personRequest3 = _fixture
            .Build<PersonAddRequest>()
            .With(temp => temp.Email, "someone_3@example.com")
            .Create();

        var personRequests = new List<PersonAddRequest>
        {
            personRequest1,
            personRequest2,
            personRequest3
        };

        var personResponseListFromAdd = new List<PersonResponse>();

        foreach (var personRequest in personRequests)
        {
            var personResponse = await _personsService.AddPerson(personRequest);
            personResponseListFromAdd.Add(personResponse);
        }

        // print person_response_list_from_add
        _testOutputHelper.WriteLine("Expected:");
        personResponseListFromAdd.ForEach(p => _testOutputHelper.WriteLine(p.ToString()));

        // Act
        var personsListFromSearch = await _personsService.GetFilteredPersons(
            nameof(Person.PersonName),
            ""
        );

        // print persons_list_from_get
        _testOutputHelper.WriteLine("Actual:");
        foreach (var personResponseFromGet in personsListFromSearch)
            _testOutputHelper.WriteLine(personResponseFromGet.ToString());

        // Assert
        foreach (var personResponseFromAdd in personResponseListFromAdd)
            Assert.Contains(personResponseFromAdd, personsListFromSearch);
    }

    // First we will add few persons, then we will search based on person name with some search string. It should return the matching persons.
    [Fact]
    public async Task GetFilteredPersons_SearchByPersonName()
    {
        // Arrange
        var countryRequest1 = _fixture.Create<CountryAddRequest>();
        var countryRequest2 = _fixture.Create<CountryAddRequest>();

        var countryResponse1 = await _countriesService.AddCountry(countryRequest1);
        var countryResponse2 = await _countriesService.AddCountry(countryRequest2);

        var personRequest1 = _fixture
            .Build<PersonAddRequest>()
            .With(temp => temp.PersonName, "Rahman")
            .With(temp => temp.Email, "someone_1@example.com")
            .With(temp => temp.CountryId, countryResponse1.CountryId)
            .Create();

        var personRequest2 = _fixture
            .Build<PersonAddRequest>()
            .With(temp => temp.PersonName, "mary")
            .With(temp => temp.Email, "someone_2@example.com")
            .With(temp => temp.CountryId, countryResponse1.CountryId)
            .Create();

        var personRequest3 = _fixture
            .Build<PersonAddRequest>()
            .With(temp => temp.PersonName, "scott")
            .With(temp => temp.Email, "someone_3@example.com")
            .With(temp => temp.CountryId, countryResponse2.CountryId)
            .Create();

        var personRequests = new List<PersonAddRequest>
        {
            personRequest1,
            personRequest2,
            personRequest3
        };

        var personResponseListFromAdd = new List<PersonResponse>();

        foreach (var personRequest in personRequests)
        {
            var personResponse = await _personsService.AddPerson(personRequest);
            personResponseListFromAdd.Add(personResponse);
        }

        // print person_response_list_from_add
        _testOutputHelper.WriteLine("Expected:");
        personResponseListFromAdd.ForEach(p => _testOutputHelper.WriteLine(p.ToString()));

        // Act
        var personsListFromSearch = await _personsService.GetFilteredPersons(
            nameof(PersonResponse.PersonName),
            "ma"
        );

        // print persons_list_from_get
        _testOutputHelper.WriteLine("Actual:");
        personsListFromSearch.ForEach(p => _testOutputHelper.WriteLine(p.ToString()));

        // Assert
        personResponseListFromAdd
            .FindAll(p => p.PersonName != null && p.PersonName.Contains("ma"))
            .ForEach(p => Assert.Contains(p, personsListFromSearch));
    }

    #endregion


    #region UpdatePerson

    // When we supply null as PersonUpdateRequest, it should throw ArgumentNullException
    [Fact]
    public async Task UpdatePerson_NullPerson()
    {
        // Arrange
        PersonUpdateRequest? personUpdateRequest = null;

        // Assert
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
        {
            // Act
            await _personsService.UpdatePerson(personUpdateRequest);
        });
    }

    // When we supply invalid person id, it should throw ArgumentException
    [Fact]
    public async Task UpdatePerson_InvalidPersonID()
    {
        // Arrange
        var personUpdateRequest = _fixture.Build<PersonUpdateRequest>().Create();

        // Assert
        await Assert.ThrowsAsync<ArgumentException>(async () =>
        {
            // Act
            await _personsService.UpdatePerson(personUpdateRequest);
        });
    }

    // When PersonName is null, it should throw ArgumentException
    [Fact]
    public async Task UpdatePerson_PersonNameIsNull()
    {
        // Arrange
        var countryRequest = _fixture.Create<CountryAddRequest>();

        var countryResponse = await _countriesService.AddCountry(countryRequest);

        var personAddRequest = _fixture
            .Build<PersonAddRequest>()
            .With(temp => temp.PersonName, "Rahman")
            .With(temp => temp.Email, "someone@example.com")
            .With(temp => temp.CountryId, countryResponse.CountryId)
            .Create();

        var personResponseFromAdd = await _personsService.AddPerson(personAddRequest);

        var personUpdateRequest = personResponseFromAdd.ToPersonUpdateRequest();
        personUpdateRequest.PersonName = null;

        // Assert
        await Assert.ThrowsAsync<ArgumentException>(async () =>
        {
            // Act
            await _personsService.UpdatePerson(personUpdateRequest);
        });
    }

    // First, add a new person and try to update the person name and email
    [Fact]
    public async Task UpdatePerson_PersonFullDetailsUpdate()
    {
        // Arrange
        var countryRequest = _fixture.Create<CountryAddRequest>();

        var countryResponse = await _countriesService.AddCountry(countryRequest);

        var personAddRequest = _fixture
            .Build<PersonAddRequest>()
            .With(temp => temp.PersonName, "Rahman")
            .With(temp => temp.Email, "someone@example.com")
            .With(temp => temp.CountryId, countryResponse.CountryId)
            .Create();

        var personResponseFromAdd = await _personsService.AddPerson(personAddRequest);

        var personUpdateRequest = personResponseFromAdd.ToPersonUpdateRequest();
        personUpdateRequest.PersonName = "William";
        personUpdateRequest.Email = "william@example.com";

        // Act
        var personResponseFromUpdate = await _personsService.UpdatePerson(personUpdateRequest);

        var personResponseFromGet = await _personsService.GetPersonByPersonId(
            personResponseFromUpdate.PersonId
        );

        // Assert
        Assert.Equal(personResponseFromGet, personResponseFromUpdate);
    }

    #endregion


    #region DeletePerson

    // If you supply an valid PersonID, it should return true
    [Fact]
    public async Task DeletePerson_ValidPersonID()
    {
        // Arrange
        var countryRequest = _fixture.Create<CountryAddRequest>();

        var countryResponse = await _countriesService.AddCountry(countryRequest);

        var personAddRequest = _fixture
            .Build<PersonAddRequest>()
            .With(temp => temp.PersonName, "Rahman")
            .With(temp => temp.Email, "someone@example.com")
            .With(temp => temp.CountryId, countryResponse.CountryId)
            .Create();

        var personResponseFromAdd = await _personsService.AddPerson(personAddRequest);

        // Act
        var isDeleted = await _personsService.DeletePerson(personResponseFromAdd.PersonId);

        // Assert
        Assert.True(isDeleted);
    }

    // If you supply an invalid PersonID, it should return false
    [Fact]
    public async Task DeletePerson_InvalidPersonID()
    {
        // Act
        var isDeleted = await _personsService.DeletePerson(Guid.NewGuid());

        // Assert
        Assert.False(isDeleted);
    }

    #endregion
}
