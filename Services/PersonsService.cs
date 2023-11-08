using Entities;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.DTO.Enums;
using Services.Helpers;

namespace Services;

public class PersonsService : IPersonsService
{
    private readonly ICountriesService _countriesService = new CountriesService();
    private readonly List<Person> _persons = new();

    public PersonResponse AddPerson(PersonAddRequest? personAddRequest)
    {
        if (personAddRequest == null) throw new ArgumentNullException(nameof(personAddRequest));

        ValidationHelper.ModelValidation(personAddRequest);

        var person = personAddRequest.ToPerson();
        person.PersonId = Guid.NewGuid();

        _persons.Add(person);

        return ConvertPersonToPersonResponse(person);
    }

    public List<PersonResponse> GetAllPersons()
    {
        return _persons.Select(ConvertPersonToPersonResponse).ToList();
    }

    public PersonResponse? GetPersonByPersonId(Guid? personId)
    {
        if (personId == null) return null;
        var person = _persons.FirstOrDefault(p => p.PersonId == personId);
        return person == null ? null : ConvertPersonToPersonResponse(person);
    }

    public List<PersonResponse> GetFilteredPersons(string searchBy, string? searchString)
    {
        var allPersons = GetAllPersons();

        var matchingPersons = allPersons;
        if (string.IsNullOrEmpty(searchString) || string.IsNullOrEmpty(searchBy)) return matchingPersons;

        matchingPersons = searchBy switch
        {
            nameof(PersonResponse.PersonName) => allPersons.Where(p =>
                    !string.IsNullOrEmpty(p.PersonName) &&
                    p.PersonName.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                .ToList(),
            nameof(PersonResponse.Email) => allPersons.Where(p =>
                    !string.IsNullOrEmpty(p.Email) &&
                    p.Email.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                .ToList(),
            nameof(PersonResponse.DateOfBirth) => allPersons.Where(p =>
                    p.DateOfBirth != null && p.DateOfBirth.Value.ToString("dd MMM yyyy")
                        .Contains(searchString, StringComparison.OrdinalIgnoreCase))
                .ToList(),
            nameof(PersonResponse.Gender) => allPersons.Where(p =>
                    p.Gender != null && p.Gender.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                .ToList(),
            nameof(PersonResponse.CountryName) => allPersons.Where(p =>
                    p.CountryName != null && p.CountryName.ToString()
                        .Contains(searchString, StringComparison.OrdinalIgnoreCase))
                .ToList(),
            nameof(PersonResponse.Address) => allPersons.Where(p =>
                    p.Address != null && p.Address.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                .ToList(),
            _ => allPersons
        };

        return matchingPersons;
    }

    public List<PersonResponse> GetSortedPersons(List<PersonResponse> allPersons, string sortBy,
        SortOrderOptions sortOrder)
    {
        if (string.IsNullOrEmpty(sortBy)) return allPersons;

        var sortedPersons = sortBy switch
        {
            nameof(PersonResponse.PersonName) => ToSortedList(allPersons, p => p.PersonName, sortOrder),
            nameof(PersonResponse.Email) => ToSortedList(allPersons, p => p.Email, sortOrder),
            nameof(PersonResponse.DateOfBirth) => ToSortedList(allPersons, p => p.DateOfBirth, sortOrder),
            nameof(PersonResponse.Gender) => ToSortedList(allPersons, p => p.Gender, sortOrder),
            nameof(PersonResponse.CountryName) => ToSortedList(allPersons, p => p.CountryName, sortOrder),
            nameof(PersonResponse.Address) => ToSortedList(allPersons, p => p.Address, sortOrder),
            _ => allPersons
        };

        return sortedPersons;
    }

    public PersonResponse UpdatePerson(PersonUpdateRequest? personUpdateRequest)
    {
        throw new NotImplementedException();
    }

    private static List<PersonResponse> ToSortedList(IEnumerable<PersonResponse> allPersons,
        Func<PersonResponse, object?> keySelector, SortOrderOptions sortOrder)
    {
        return (sortOrder.Equals(SortOrderOptions.ASC)
            ? allPersons.OrderBy(keySelector)
            : allPersons.OrderByDescending(keySelector)).ToList();
    }

    private PersonResponse ConvertPersonToPersonResponse(Person person)
    {
        var personResponse = person.ToPersonResponse();
        personResponse.CountryName = _countriesService.GetCountryByCountryId(person.CountryId)?.CountryName;
        return personResponse;
    }
}