using ServiceContracts.DTO;
using ServiceContracts.Enums;

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
    Task<PersonResponse> AddPerson(PersonAddRequest? personAddRequest);

    /// <summary>
    ///     Returns all persons
    /// </summary>
    /// <returns>Returns a list of objects of PersonResponse type</returns>
    Task<List<PersonResponse>> GetAllPersons();

    /// <summary>
    ///     Returns the person object based on the PersonId
    /// </summary>
    /// <param name="personId">Person id to search</param>
    /// <returns>Returns matching person object</returns>
    Task<PersonResponse?> GetPersonByPersonId(Guid? personId);

    /// <summary>
    ///     Returns all person objects that matches with the given search field and search string
    /// </summary>
    /// <param name="searchBy">Search field to search</param>
    /// <param name="searchString">Search string to search</param>
    /// <returns>Returns all matching persons based on the given search field and search string</returns>
    Task<List<PersonResponse>> GetFilteredPersons(string searchBy, string? searchString);

    /// <summary>
    ///     Returns sorted list of persons
    /// </summary>
    /// <param name="allPersons">List of persons to sort</param>
    /// <param name="sortBy">Name of the property</param>
    /// <param name="sortOrder">ASC or DESC</param>
    /// <returns>Return sorted person as PersonResponse list</returns>
    Task<List<PersonResponse>> GetSortedPersons(List<PersonResponse> allPersons, string sortBy,
        SortOrderOptions sortOrder);

    /// <summary>
    ///     Updates the specified person details based on given person id
    /// </summary>
    /// <param name="personUpdateRequest">Person details to update, including person id</param>
    /// <returns>The person updated</returns>
    Task<PersonResponse> UpdatePerson(PersonUpdateRequest? personUpdateRequest);

    /// <summary>
    ///     Deletes a person from the list based on the given person id
    /// </summary>
    /// <param name="personId">PersonId to be deleted</param>
    /// <returns>Returns true if the deletion is successful otherwise false</returns>
    Task<bool> DeletePerson(Guid? personId);
}