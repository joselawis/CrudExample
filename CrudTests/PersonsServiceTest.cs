using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.DTO.Enums;
using Services;

namespace CrudTests;

public class PersonsServiceTest
{
    private readonly IPersonsService _personsService = new PersonsService();

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
        Assert.Throws<ArgumentNullException>(() => _personsService.AddPerson(personAddRequest));
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
    
    #endregion
}