using Entities;
using Microsoft.EntityFrameworkCore;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Services.Helpers;

namespace Services;

public class PersonsService : IPersonsService
{
    private readonly ICountriesService _countriesService;
    private readonly PersonsDbContext _db;

    public PersonsService(PersonsDbContext personsDbContext, ICountriesService countriesService)
    {
        _db = personsDbContext;
        _countriesService = countriesService;
    }

    public async Task<PersonResponse> AddPerson(PersonAddRequest? personAddRequest)
    {
        if (personAddRequest == null)
            throw new ArgumentNullException(nameof(personAddRequest));

        ValidationHelper.ModelValidation(personAddRequest);

        var person = personAddRequest.ToPerson();
        person.PersonId = Guid.NewGuid();

        _db.Persons.Add(person);
        await _db.SaveChangesAsync();
        //_db.sp_InsertPerson(person);

        return person.ToPersonResponse();
    }

    public async Task<List<PersonResponse>> GetAllPersons()
    {
        var persons = await _db.Persons.Include("Country").ToListAsync();

        return persons.Select(person => person.ToPersonResponse()).ToList();
        // return _db.sp_GetAllPersons().Select(person => person.ToPersonResponse()).ToList();
    }

    public async Task<PersonResponse?> GetPersonByPersonId(Guid? personId)
    {
        if (personId == null)
            return null;
        var person = await _db.Persons
            .Include("Country")
            .FirstOrDefaultAsync(p => p.PersonId == personId);
        return person?.ToPersonResponse();
    }

    public async Task<List<PersonResponse>> GetFilteredPersons(
        string searchBy,
        string? searchString
    )
    {
        var allPersons = await GetAllPersons();
        var matchingPersons = allPersons;

        if (string.IsNullOrEmpty(searchString) || string.IsNullOrEmpty(searchBy))
            return matchingPersons;

        matchingPersons = searchBy switch
        {
            nameof(PersonResponse.PersonName)
                => allPersons
                    .Where(
                        p =>
                            !string.IsNullOrEmpty(p.PersonName)
                            && p.PersonName.Contains(
                                searchString,
                                StringComparison.OrdinalIgnoreCase
                            )
                    )
                    .ToList(),
            nameof(PersonResponse.Email)
                => allPersons
                    .Where(
                        p =>
                            !string.IsNullOrEmpty(p.Email)
                            && p.Email.Contains(searchString, StringComparison.OrdinalIgnoreCase)
                    )
                    .ToList(),
            nameof(PersonResponse.DateOfBirth)
                => allPersons
                    .Where(
                        p =>
                            p.DateOfBirth != null
                            && p.DateOfBirth
                                .Value
                                .ToString("dd MMM yyyy")
                                .Contains(searchString, StringComparison.OrdinalIgnoreCase)
                    )
                    .ToList(),
            nameof(PersonResponse.Gender)
                => allPersons
                    .Where(
                        p =>
                            p.Gender != null
                            && p.Gender.Equals(searchString, StringComparison.OrdinalIgnoreCase)
                    )
                    .ToList(),
            nameof(PersonResponse.CountryId)
                => allPersons
                    .Where(
                        p =>
                            p.CountryName != null
                            && p.CountryName
                                .ToString()
                                .Contains(searchString, StringComparison.OrdinalIgnoreCase)
                    )
                    .ToList(),
            nameof(PersonResponse.Address)
                => allPersons
                    .Where(
                        p =>
                            p.Address != null
                            && p.Address.Contains(searchString, StringComparison.OrdinalIgnoreCase)
                    )
                    .ToList(),
            _ => allPersons
        };

        return matchingPersons;
    }

    public Task<List<PersonResponse>> GetSortedPersons(
        List<PersonResponse> allPersons,
        string sortBy,
        SortOrderOptions sortOrder
    )
    {
        if (string.IsNullOrEmpty(sortBy))
            return Task.FromResult(allPersons);

        var sortedPersons = sortBy switch
        {
            nameof(PersonResponse.PersonName)
                => ToSortedList(allPersons, p => p.PersonName, sortOrder),
            nameof(PersonResponse.Email) => ToSortedList(allPersons, p => p.Email, sortOrder),
            nameof(PersonResponse.DateOfBirth)
                => ToSortedList(allPersons, p => p.DateOfBirth, sortOrder),
            nameof(PersonResponse.Gender) => ToSortedList(allPersons, p => p.Gender, sortOrder),
            nameof(PersonResponse.CountryName)
                => ToSortedList(allPersons, p => p.CountryName, sortOrder),
            nameof(PersonResponse.Address) => ToSortedList(allPersons, p => p.Address, sortOrder),
            _ => allPersons
        };

        return Task.FromResult(sortedPersons);
    }

    public async Task<PersonResponse> UpdatePerson(PersonUpdateRequest? personUpdateRequest)
    {
        if (personUpdateRequest == null)
            throw new ArgumentNullException(nameof(personUpdateRequest));

        ValidationHelper.ModelValidation(personUpdateRequest);

        var matchingPerson = await _db.Persons.FirstOrDefaultAsync(
            p => p.PersonId == personUpdateRequest.PersonId
        );
        if (matchingPerson == null)
            throw new ArgumentException("Person not found");

        matchingPerson.PersonName = personUpdateRequest.PersonName;
        matchingPerson.Email = personUpdateRequest.Email;
        matchingPerson.DateOfBirth = personUpdateRequest.DateOfBirth;
        matchingPerson.Gender = personUpdateRequest.Gender?.ToString();
        matchingPerson.CountryId = personUpdateRequest.CountryId;
        matchingPerson.Address = personUpdateRequest.Address;
        matchingPerson.ReceiveNewsLetters = personUpdateRequest.ReceiveNewsLetters;

        await _db.SaveChangesAsync();

        return matchingPerson.ToPersonResponse();
    }

    public async Task<bool> DeletePerson(Guid? personId)
    {
        if (personId == null)
            throw new ArgumentNullException(nameof(personId));
        _db.Persons.Remove(_db.Persons.First(p => p.PersonId == personId));
        await _db.SaveChangesAsync();
        return true;
    }

    private static List<PersonResponse> ToSortedList(
        IEnumerable<PersonResponse> allPersons,
        Func<PersonResponse, object?> keySelector,
        SortOrderOptions sortOrder
    )
    {
        return (
            sortOrder.Equals(SortOrderOptions.Asc)
                ? allPersons.OrderBy(keySelector)
                : allPersons.OrderByDescending(keySelector)
        ).ToList();
    }
}
