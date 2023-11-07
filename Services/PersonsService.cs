using Entities;
using ServiceContracts;
using ServiceContracts.DTO;
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
            nameof(Person.PersonName) => allPersons.Where(p =>
                    !string.IsNullOrEmpty(p.PersonName) &&
                    p.PersonName.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                .ToList(),
            nameof(Person.Email) => allPersons.Where(p =>
                    !string.IsNullOrEmpty(p.Email) &&
                    p.Email.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                .ToList(),
            nameof(Person.DateOfBirth) => allPersons.Where(p =>
                    p.DateOfBirth != null && p.DateOfBirth.Value.ToString("dd MMM yyyy")
                        .Contains(searchString, StringComparison.OrdinalIgnoreCase))
                .ToList(),
            nameof(Person.Gender) => allPersons.Where(p =>
                    p.Gender != null && p.Gender.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                .ToList(),
            nameof(Person.CountryId) => allPersons.Where(p =>
                    p.CountryId != null && p.CountryId.Value.ToString()
                        .Contains(searchString, StringComparison.OrdinalIgnoreCase))
                .ToList(),
            nameof(Person.Address) => allPersons.Where(p =>
                    p.Address != null && p.Address.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                .ToList(),
            _ => allPersons
        };

        return matchingPersons;
    }

    private PersonResponse ConvertPersonToPersonResponse(Person person)
    {
        var personResponse = person.ToPersonResponse();
        personResponse.CountryName = _countriesService.GetCountryByCountryId(person.CountryId)?.CountryName;
        return personResponse;
    }
}