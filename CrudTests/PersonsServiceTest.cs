using System.Linq.Expressions;
using AutoFixture;
using Entities;
using FluentAssertions;
using Moq;
using RepositoryContracts;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Services;
using Xunit.Abstractions;

namespace CrudTests;

public class PersonsServiceTest
{
    private readonly IFixture _fixture;

    private readonly Mock<IPersonsRepository> _personsRepositoryMock;

    // private fields
    private readonly IPersonsService _personsService;

    private readonly ITestOutputHelper _testOutputHelper;

    // constructor
    public PersonsServiceTest(ITestOutputHelper testOutputHelper)
    {
        _fixture = new Fixture();
        _personsRepositoryMock = new Mock<IPersonsRepository>();
        var personsRepository = _personsRepositoryMock.Object;

        // Create services based on mocked DbContext object
        _personsService = new PersonsService(personsRepository);

        _testOutputHelper = testOutputHelper;
    }

    #region GetSortedPersons

    // When we sort based on PersonName in DESC, it should return persons list in descending order
    [Fact]
    public async Task GetSortedPersons_ToBeSuccessful()
    {
        // Arrange
        var persons = new List<Person>
        {
            _fixture
                .Build<Person>()
                .With(temp => temp.Email, "someone_1@example.com")
                .With(temp => temp.Country, null as Country)
                .Create(),
            _fixture
                .Build<Person>()
                .With(temp => temp.Email, "someone_2@example.com")
                .With(temp => temp.Country, null as Country)
                .Create(),
            _fixture
                .Build<Person>()
                .With(temp => temp.Email, "someone_3@example.com")
                .With(temp => temp.Country, null as Country)
                .Create()
        };

        var personResponseListExpected = persons.Select(temp => temp.ToPersonResponse()).ToList();

        _personsRepositoryMock.Setup(temp => temp.GetAllPersons()).ReturnsAsync(persons);

        // print person_response_list_from_add
        _testOutputHelper.WriteLine("Expected:");
        personResponseListExpected.ForEach(p => _testOutputHelper.WriteLine(p.ToString()));

        var allPersons = await _personsService.GetAllPersons();

        // Act
        var personsListFromSort = await _personsService.GetSortedPersons(
            allPersons,
            nameof(Person.PersonName),
            SortOrderOptions.Desc
        );

        // print persons_list_from_get
        _testOutputHelper.WriteLine("Actual:");
        personsListFromSort.ForEach(p => _testOutputHelper.WriteLine(p.ToString()));

        // Assert
        personsListFromSort.Should().BeInDescendingOrder(temp => temp.PersonName);
    }

    #endregion


    #region AddPerson

    // When we supply null value as PersonAddRequest, it should throw ArgumentNullException
    [Fact]
    public async Task AddPerson_NullPerson_ToBeArgumentNullException()
    {
        // Arrange
        PersonAddRequest? personAddRequest = null;

        // Act
        var action = async () =>
        {
            await _personsService.AddPerson(personAddRequest);
        };
        await action.Should().ThrowAsync<ArgumentNullException>();
    }

    //When we supply null value as PersonName, it should throw ArgumentException
    [Fact]
    public async Task AddPerson_PersonNameIsNull_ToBeArgumentException()
    {
        // Arrange
        var personAddRequest = _fixture
            .Build<PersonAddRequest>()
            .With(temp => temp.PersonName, null as string)
            .Create();

        var person = personAddRequest.ToPerson();

        _personsRepositoryMock
            .Setup(temp => temp.AddPerson(It.IsAny<Person>()))
            .ReturnsAsync(person);

        // Assert
        var action = async () => await _personsService.AddPerson(personAddRequest);
        await action.Should().ThrowAsync<ArgumentException>();
    }

    //When we supply proper person details, it should insert the person into the persons list; and it should return an object of PersonResponse, which includes with the newly generated person id
    [Fact]
    public async Task AddPerson_FullPersonDetails_ToBeSuccessful()
    {
        // Arrange
        var personAddRequest = _fixture
            .Build<PersonAddRequest>()
            .With(temp => temp.Email, "someone@example.com")
            .Create();

        var person = personAddRequest.ToPerson();
        var personResponseExpected = person.ToPersonResponse();

        // If we supply any argument value to the AddPerson method, it should return the same return value
        _personsRepositoryMock
            .Setup(temp => temp.AddPerson(It.IsAny<Person>()))
            .ReturnsAsync(person);

        // Act
        var personResponseFromAdd = await _personsService.AddPerson(personAddRequest);
        personResponseExpected.PersonId = personResponseFromAdd.PersonId;

        // Assert
        personResponseFromAdd.PersonId.Should().NotBe(Guid.Empty);
        personResponseFromAdd.Should().Be(personResponseExpected);
    }

    #endregion


    #region GetPersonByPersonID

    // If we supply null as PersonID, it should return null as PersonResponse
    [Fact]
    public async Task GetPersonByPersonID_NullPersonID_ToBeNull()
    {
        // Arrange
        Guid? personId = null;

        // Act
        var personResponseFromGet = await _personsService.GetPersonByPersonId(personId);

        // Assert
        personResponseFromGet.Should().BeNull();
    }

    // If we supply a valid person id, it should return the valid person details as PersonResponse object
    [Fact]
    public async Task GetPersonByPersonID_WithPersonID_ToBeSuccessful()
    {
        // Arrange
        var person = _fixture
            .Build<Person>()
            .With(temp => temp.Email, "email@sample.com")
            .With(temp => temp.Country, null as Country)
            .Create();

        var personResponseExpected = person.ToPersonResponse();

        _personsRepositoryMock
            .Setup(temp => temp.GetPersonByPersonId(It.IsAny<Guid>()))
            .ReturnsAsync(person);

        // Act
        var personResponseFromGet = await _personsService.GetPersonByPersonId(person.PersonId);

        // Assert
        personResponseFromGet.Should().Be(personResponseExpected);
    }

    #endregion


    #region GetAllPersons

    // The GetAllPersons() should return an empty list by default
    [Fact]
    public async Task GetAllPersons_EmptyList()
    {
        var persons = new List<Person>();
        _personsRepositoryMock.Setup(temp => temp.GetAllPersons()).ReturnsAsync(persons);

        // Act
        var personsFromGet = await _personsService.GetAllPersons();

        // Assert
        personsFromGet.Should().BeEmpty();
    }

    // First, we will add few persons; and then when we call GetAllPersons(), it should return the same persons that were added
    [Fact]
    public async Task GetAllPersons_AddFewPersons_ToBeSuccessful()
    {
        // Arrange
        var persons = new List<Person>
        {
            _fixture
                .Build<Person>()
                .With(temp => temp.Email, "someone_1@example.com")
                .With(temp => temp.Country, null as Country)
                .Create(),
            _fixture
                .Build<Person>()
                .With(temp => temp.Email, "someone_2@example.com")
                .With(temp => temp.Country, null as Country)
                .Create(),
            _fixture
                .Build<Person>()
                .With(temp => temp.Email, "someone_3@example.com")
                .With(temp => temp.Country, null as Country)
                .Create()
        };

        var personResponseListExpected = persons.Select(temp => temp.ToPersonResponse()).ToList();

        // print person_response_list_from_add
        _testOutputHelper.WriteLine("Expected:");
        personResponseListExpected.ForEach(p => _testOutputHelper.WriteLine(p.ToString()));

        _personsRepositoryMock.Setup(temp => temp.GetAllPersons()).ReturnsAsync(persons);
        // Act
        var personsListFromGet = await _personsService.GetAllPersons();

        // print persons_list_from_get
        _testOutputHelper.WriteLine("Actual:");
        personsListFromGet.ForEach(p => _testOutputHelper.WriteLine(p.ToString()));

        // Assert
        personsListFromGet.Should().BeEquivalentTo(personResponseListExpected);
    }

    #endregion


    #region GetFilteredPersons

    // If the search text is empty and search by is "PersonName", it should return all persons
    [Fact]
    public async Task GetFilteredPersons_EmptySearchText_ToBeSuccessful()
    {
        var persons = new List<Person>
        {
            _fixture
                .Build<Person>()
                .With(temp => temp.Email, "someone_1@example.com")
                .With(temp => temp.Country, null as Country)
                .Create(),
            _fixture
                .Build<Person>()
                .With(temp => temp.Email, "someone_2@example.com")
                .With(temp => temp.Country, null as Country)
                .Create(),
            _fixture
                .Build<Person>()
                .With(temp => temp.Email, "someone_3@example.com")
                .With(temp => temp.Country, null as Country)
                .Create()
        };

        var personResponseListExpected = persons.Select(temp => temp.ToPersonResponse()).ToList();

        // print person_response_list_from_add
        _testOutputHelper.WriteLine("Expected:");
        personResponseListExpected.ForEach(p => _testOutputHelper.WriteLine(p.ToString()));

        _personsRepositoryMock
            .Setup(temp => temp.GetFilteredPersons(It.IsAny<Expression<Func<Person, bool>>>()))
            .ReturnsAsync(persons);

        // Act
        var personsListFromSearch = await _personsService.GetFilteredPersons(
            nameof(Person.PersonName),
            ""
        );

        // print persons_list_from_get
        _testOutputHelper.WriteLine("Actual:");
        personsListFromSearch.ForEach(p => _testOutputHelper.WriteLine(p.ToString()));

        // Assert
        personsListFromSearch.Should().BeEquivalentTo(personResponseListExpected);
    }

    // First we will add few persons, then we will search based on person name with some search string. It should return the matching persons.
    [Fact]
    public async Task GetFilteredPersons_SearchByPersonName_ToBeSuccessful()
    {
        // Arrange
        var persons = new List<Person>
        {
            _fixture
                .Build<Person>()
                .With(temp => temp.Email, "someone_1@example.com")
                .With(temp => temp.Country, null as Country)
                .Create(),
            _fixture
                .Build<Person>()
                .With(temp => temp.Email, "someone_2@example.com")
                .With(temp => temp.Country, null as Country)
                .Create(),
            _fixture
                .Build<Person>()
                .With(temp => temp.Email, "someone_3@example.com")
                .With(temp => temp.Country, null as Country)
                .Create()
        };

        var personResponseListExpected = persons.Select(temp => temp.ToPersonResponse()).ToList();

        // print person_response_list_from_add
        _testOutputHelper.WriteLine("Expected:");
        personResponseListExpected.ForEach(p => _testOutputHelper.WriteLine(p.ToString()));

        _personsRepositoryMock
            .Setup(temp => temp.GetFilteredPersons(It.IsAny<Expression<Func<Person, bool>>>()))
            .ReturnsAsync(persons);

        // Act
        var personsListFromSearch = await _personsService.GetFilteredPersons(
            nameof(Person.PersonName),
            "sa"
        );

        // print persons_list_from_get
        _testOutputHelper.WriteLine("Actual:");
        personsListFromSearch.ForEach(p => _testOutputHelper.WriteLine(p.ToString()));

        // Assert
        personsListFromSearch.Should().BeEquivalentTo(personResponseListExpected);
    }

    #endregion


    #region UpdatePerson

    // When we supply null as PersonUpdateRequest, it should throw ArgumentNullException
    [Fact]
    public async Task UpdatePerson_NullPerson_ToBeArgumentNullException()
    {
        // Arrange
        PersonUpdateRequest? personUpdateRequest = null;

        // Assert
        var action = async () =>
        {
            // Act
            await _personsService.UpdatePerson(personUpdateRequest);
        };
        await action.Should().ThrowAsync<ArgumentNullException>();
    }

    // When we supply invalid person id, it should throw ArgumentException
    [Fact]
    public async Task UpdatePerson_InvalidPersonID_ToBeArgumentException()
    {
        // Arrange
        var personUpdateRequest = _fixture.Build<PersonUpdateRequest>().Create();

        // Assert
        var action = async () =>
        {
            // Act
            await _personsService.UpdatePerson(personUpdateRequest);
        };
        await action.Should().ThrowAsync<ArgumentException>();
    }

    // When PersonName is null, it should throw ArgumentException
    [Fact]
    public async Task UpdatePerson_PersonNameIsNull_ToBeArgumentException()
    {
        // Arrange
        var person = _fixture
            .Build<Person>()
            .With(temp => temp.PersonName, null as string)
            .With(temp => temp.Email, "someone@example.com")
            .With(temp => temp.Gender, "Male")
            .With(temp => temp.Country, null as Country)
            .Create();

        var personResponseExpected = person.ToPersonResponse();

        var personUpdateRequest = personResponseExpected.ToPersonUpdateRequest();

        // Assert
        var action = async () =>
        {
            // Act
            await _personsService.UpdatePerson(personUpdateRequest);
        };
        await action.Should().ThrowAsync<ArgumentException>();
    }

    // First, add a new person and try to update the person name and email
    [Fact]
    public async Task UpdatePerson_PersonFullDetailsUpdate_ToBeSuccessful()
    {
        // Arrange
        var person = _fixture
            .Build<Person>()
            .With(temp => temp.Email, "someone@example.com")
            .With(temp => temp.Country, null as Country)
            .With(temp => temp.Gender, "Male")
            .Create();

        var personResponseExpected = person.ToPersonResponse();

        var personUpdateRequest = personResponseExpected.ToPersonUpdateRequest();

        _personsRepositoryMock
            .Setup(temp => temp.UpdatePerson(It.IsAny<Person>()))
            .ReturnsAsync(person);

        _personsRepositoryMock
            .Setup(temp => temp.GetPersonByPersonId(It.IsAny<Guid>()))
            .ReturnsAsync(person);

        // Act
        var personResponseFromUpdate = await _personsService.UpdatePerson(personUpdateRequest);

        // Assert
        personResponseFromUpdate.Should().Be(personResponseExpected);
    }

    #endregion


    #region DeletePerson

    // If you supply an valid PersonID, it should return true
    [Fact]
    public async Task DeletePerson_ValidPersonID_ToBeSuccessful()
    {
        // Arrange
        var person = _fixture
            .Build<Person>()
            .With(temp => temp.PersonName, "Rahman")
            .With(temp => temp.Email, "someone@example.com")
            .With(temp => temp.Country, null as Country)
            .With(temp => temp.Gender, "Male")
            .Create();

        _personsRepositoryMock
            .Setup(temp => temp.DeletePersonByPersonId(It.IsAny<Guid>()))
            .ReturnsAsync(true);

        _personsRepositoryMock
            .Setup(temp => temp.GetPersonByPersonId(It.IsAny<Guid>()))
            .ReturnsAsync(person);

        // Act
        var isDeleted = await _personsService.DeletePerson(person.PersonId);

        // Assert
        isDeleted.Should().BeTrue();
    }

    // If you supply an invalid PersonID, it should return false
    [Fact]
    public async Task DeletePerson_InvalidPersonID()
    {
        // Act
        var isDeleted = await _personsService.DeletePerson(Guid.NewGuid());

        // Assert
        isDeleted.Should().BeFalse();
    }

    #endregion
}
