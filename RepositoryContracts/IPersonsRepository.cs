using System.Linq.Expressions;
using Entities;

namespace RepositoryContracts;

/// <summary>
///     Represents data access logic for managing Persons entities
/// </summary>
public interface IPersonsRepository
{
    /// <summary>
    ///     Adds a Person object to the data store
    /// </summary>
    /// <param name="person">Person object to add</param>
    /// <returns>Returns the person object after adding it to the table</returns>
    Task<Person> AddPerson(Person person);

    /// <summary>
    ///     Returns all persons in the data store
    /// </summary>
    /// <returns>List of person objects from table</returns>
    Task<List<Person>> GetAllPersons();

    /// <summary>
    ///     Returns a person object based on the given person id
    /// </summary>
    /// <param name="personId">PersonId to search</param>
    /// <returns>A person object or null</returns>
    Task<Person?> GetPersonByPersonId(Guid personId);

    /// <summary>
    ///     Returns all persons in the data store based on the given expression
    /// </summary>
    /// <param name="predicate">LINQ expression to check</param>
    /// <returns>All matching persons with given condition</returns>
    Task<List<Person>> GetFilteredPersons(Expression<Func<Person, bool>> predicate);

    /// <summary>
    ///     Deletes a person from the data store based on the given person id
    /// </summary>
    /// <param name="personId">PersonId to search</param>
    /// <returns>Returns true if deletion is successful; otherwise false</returns>
    Task<bool> DeletePersonByPersonId(Guid personId);

    /// <summary>
    ///     Updates a person object based on the given person id
    /// </summary>
    /// <param name="person">Person object to update</param>
    /// <returns>Returns the updated person object</returns>
    Task<Person> UpdatePerson(Person person);
}
