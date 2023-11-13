using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Services;
using Xunit.Abstractions;

namespace CrudTests;

public class PersonsServiceTest
{
    private readonly ICountriesService _countriesService = new CountriesService(false);
    private readonly IPersonsService _personsService = new PersonsService(false);
    private readonly ITestOutputHelper _testOutputHelper;

    public PersonsServiceTest(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    #region GetSortedPersons

    // When we sort based on PersonName in DESC, it should return persons list in descending order
    [Fact]
    public void GetSortedPersons()
    {
        var allPersons = AddDummyPersons();
        var sortedAllPersons = allPersons.OrderByDescending(p => p.PersonName).ToList();

        // Print expected
        _testOutputHelper.WriteLine("Expected:");
        allPersons.ForEach(p => _testOutputHelper.WriteLine(p.ToString()));


        var personsSorted =
            _personsService.GetSortedPersons(allPersons, nameof(PersonResponse.PersonName), SortOrderOptions.Desc);
        // Print actual
        _testOutputHelper.WriteLine("Actual:");
        personsSorted.ForEach(p => _testOutputHelper.WriteLine(p.ToString()));

        Assert.Equal(3, personsSorted.Count);
        Assert.True(sortedAllPersons.Zip(personsSorted, (a, b) => a.Equals(b)).All(match => match));
        Assert.Equal(sortedAllPersons, personsSorted);
    }

    #endregion

    private List<PersonResponse> AddDummyPersons()
    {
        var countryAddRequest1 = new CountryAddRequest { CountryName = "USA" };
        var countryAddRequest2 = new CountryAddRequest { CountryName = "España" };
        var countryAddRequest3 = new CountryAddRequest { CountryName = "India" };

        var countryAddResponse1 = _countriesService.AddCountry(countryAddRequest1);
        var countryAddResponse2 = _countriesService.AddCountry(countryAddRequest2);
        var countryAddResponse3 = _countriesService.AddCountry(countryAddRequest3);

        var personAddRequest1 = new PersonAddRequest
        {
            PersonName = "John", Email = "person@example.com", Address = "Address1",
            CountryId = countryAddResponse1.CountryId, Gender = GenderOptions.Male, DateOfBirth = DateTime.Now,
            ReceiveNewsLetters = true
        };
        var personAddRequest2 = new PersonAddRequest
        {
            PersonName = "Maria", Email = "person@example.com", Address = "Address2",
            Gender = GenderOptions.Female, DateOfBirth = DateTime.Now, CountryId = countryAddResponse2.CountryId,
            ReceiveNewsLetters = true
        };
        var personAddRequest3 = new PersonAddRequest
        {
            PersonName = "Mario", Email = "person@example.com", Address = "Address3",
            Gender = GenderOptions.Male, DateOfBirth = DateTime.Now, CountryId = countryAddResponse3.CountryId,
            ReceiveNewsLetters = true
        };

        var personsRequest = new List<PersonAddRequest>
        {
            personAddRequest1, personAddRequest2, personAddRequest3
        };
        var allPersons = personsRequest.Select(p => _personsService.AddPerson(p)).ToList();
        return allPersons;
    }

    #region UpdatePerson

    // When we supply null as PersonUpdateRequest, it should throw ArgumentNullException
    [Fact]
    public void UpdatePerson_NullPersonUpdateRequest()
    {
        Assert.Throws<ArgumentNullException>(() => _personsService.UpdatePerson(null));
    }

    // When we supply invalid person id, it should throw ArgumentException
    [Fact]
    public void UpdatePerson_InvalidPersonId()
    {
        var personUpdateRequest = new PersonUpdateRequest { PersonId = Guid.NewGuid() };
        Assert.Throws<ArgumentException>(() => _personsService.UpdatePerson(personUpdateRequest));
    }

    // When PersonName is null, it should throw ArgumentException
    [Fact]
    public void UpdatePerson_NullPersonName()
    {
        var allPersons = AddDummyPersons();
        var personUpdateRequest = allPersons[0].ToPersonUpdateRequest();
        personUpdateRequest.PersonName = null;
        Assert.Throws<ArgumentException>(() => _personsService.UpdatePerson(personUpdateRequest));
    }

    // When we supply a valid person update request, it should update the person
    [Fact]
    public void UpdatePerson_ValidPersonUpdateRequest()
    {
        var allPersons = AddDummyPersons();
        var personUpdateRequest = allPersons[0].ToPersonUpdateRequest();
        personUpdateRequest.PersonName = "Juanito";
        var updatedPerson = _personsService.UpdatePerson(personUpdateRequest);
        Assert.Equal(personUpdateRequest.PersonName, updatedPerson.PersonName);
    }

    #endregion

    #region GetFilteredPersons

    // If the search text is empty and search by is "PersonName", it should return all the persons
    [Fact]
    public void GetFilteredPersons_EmptySearchText()
    {
        var allPersons = AddDummyPersons();

        // Print expected
        _testOutputHelper.WriteLine("Expected:");
        allPersons.ForEach(p => _testOutputHelper.WriteLine(p.ToString()));


        var personsFromFilteredSearch = _personsService.GetFilteredPersons(nameof(PersonResponse.PersonName), "");
        // Print actual
        _testOutputHelper.WriteLine("Actual:");
        personsFromFilteredSearch.ForEach(p => _testOutputHelper.WriteLine(p.ToString()));

        Assert.Equal(3, personsFromFilteredSearch.Count);
        Assert.Equal(allPersons, personsFromFilteredSearch);
        allPersons.ForEach(p => Assert.Contains(p, personsFromFilteredSearch));
    }

    // First we will add few persons, then we will search based on person name with some search string. It should return the matching persons.
    [Fact]
    public void GetFilteredPersons_SearchByPersonName()
    {
        var allPersons = AddDummyPersons();

        // Print expected
        _testOutputHelper.WriteLine("Expected:");
        allPersons.ForEach(p => _testOutputHelper.WriteLine(p.ToString()));

        var personsFromFilteredSearch = _personsService.GetFilteredPersons(nameof(PersonResponse.PersonName), "ma");
        // Print actual
        _testOutputHelper.WriteLine("Actual:");
        personsFromFilteredSearch.ForEach(p => _testOutputHelper.WriteLine(p.ToString()));

        Assert.Equal(2, personsFromFilteredSearch.Count);
        allPersons.FindAll(p => p.PersonName != null && p.PersonName.Contains("ma"))
            .ForEach(p => Assert.Contains(p, personsFromFilteredSearch));
    }

    #endregion

    #region GetPersonByPersonId

    // If we supply null value as PersonId, it should return null as PersonResponse
    [Fact]
    public void GetPersonByPersonId_NullPersonId()
    {
        var personId = Guid.Empty;

        var personResponse = _personsService.GetPersonByPersonId(personId);

        Assert.Null(personResponse);
    }

    // If we supply a valid PersonId, it should return a PersonResponse
    [Fact]
    public void GetPersonByPersonId_ValidPersonId()
    {
        var countryAddRequest = new CountryAddRequest
        {
            CountryName = "CountryName"
        };
        var countryResponse = _countriesService.AddCountry(countryAddRequest);

        var personAddRequest = new PersonAddRequest
        {
            PersonName = "PersonName",
            Email = "person@example.com",
            Address = "Address",
            CountryId = countryResponse.CountryId,
            DateOfBirth = DateTime.Now,
            Gender = GenderOptions.Male,
            ReceiveNewsLetters = true
        };
        var addPersonResponse = _personsService.AddPerson(personAddRequest);

        var getPersonResponse = _personsService.GetPersonByPersonId(addPersonResponse.PersonId);

        Assert.Equal(addPersonResponse, getPersonResponse);
    }

    #endregion

    #region AddPerson

    // When we supply null value as PersonAddRequest, it should throw ArgumentNullException
    [Fact]
    public void AddPerson_NullPerson()
    {
        Assert.Throws<ArgumentNullException>(() => _personsService.AddPerson(null));
    }

    // When we supply null value as PersonName, it should throw ArgumentException
    [Fact]
    public void AddPerson_PersonNameIsNull()
    {
        var personAddRequest = new PersonAddRequest
        {
            PersonName = null
        };
        Assert.Throws<ArgumentException>(() => _personsService.AddPerson(personAddRequest));
    }

    // When we supply proper person details, it should insert the person into the persons list; and it should return an object of PersonResponse, with newly generated PersonId
    [Fact]
    public void AddPerson_ProperPersonDetails()
    {
        var personAddRequest = new PersonAddRequest
        {
            PersonName = "John",
            Email = "john@gmail.com",
            Address = "Fake street 123",
            CountryId = Guid.NewGuid(),
            Gender = GenderOptions.Male,
            DateOfBirth = DateTime.Now.AddYears(-18),
            ReceiveNewsLetters = true
        };

        var response = _personsService.AddPerson(personAddRequest);

        var personsList = _personsService.GetAllPersons();

        Assert.True(response.PersonId != Guid.Empty);
        Assert.Contains(response, personsList);
    }

    #endregion

    #region GetAllPersons

    // The list of persons should be empty by default
    [Fact]
    public void GetAllPersons_EmptyList()
    {
        var persons = _personsService.GetAllPersons();

        Assert.Empty(persons);
    }

    // First, we will add few Persons, then when we call GetAllPersons, it should return the list of Persons
    [Fact]
    public void GetAllPersons_AddFewPersons()
    {
        var allPersons = AddDummyPersons();

        // Print expected
        _testOutputHelper.WriteLine("Expected:");
        allPersons.ForEach(p => _testOutputHelper.WriteLine(p.ToString()));

        var personsFromGetAllPersons = _personsService.GetAllPersons();

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
    public void DeletePerson_InvalidPersonId()
    {
        var result = _personsService.DeletePerson(Guid.NewGuid());

        Assert.False(result);
    }

    // If you supply a null personId, it should throw ArgumentNullException
    [Fact]
    public void DeletePerson_PersonIdIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => _personsService.DeletePerson(null));
    }

    // If you supply a valid PersonId, it should return true
    [Fact]
    public void DeletePerson_ValidPersonId()
    {
        var allPersons = AddDummyPersons();
        var personId = allPersons[0].PersonId;

        var result = _personsService.DeletePerson(personId);

        Assert.True(result);
    }

    #endregion
}