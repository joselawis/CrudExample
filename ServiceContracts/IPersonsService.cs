using ServiceContracts.DTO;

namespace ServiceContracts;

/// <summary>
///     Represents business logic for manipulation Person entity
/// </summary>
public interface IPersonsService
{
    /// <summary>
    ///     Adds a new Person into a list of persons
    /// </summary>
    /// <param name="personAddRequest">Person to add</param>
    /// <returns>Returns the same person details, along with newly generated PersonId</returns>
    PersonResponse AddPerson(PersonAddRequest? personAddRequest);

    /// <summary>
    ///     Returns all persons
    /// </summary>
    /// <returns>Returns a list of objects of PersonResponse type</returns>
    List<PersonResponse> GetAllPersons();
}