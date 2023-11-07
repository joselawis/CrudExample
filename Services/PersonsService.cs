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

        switch (searchBy)
        {
            case nameof(Person.PersonName):
                matchingPersons = allPersons.Where(p =>
                    !string.IsNullOrEmpty(p.PersonName) &&
                    p.PersonName.Contains(searchString, StringComparison.OrdinalIgnoreCase)).ToList();
                break;
            case nameof(Person.Email):
                matchingPersons = allPersons.Where(p =>
                    !string.IsNullOrEmpty(p.Email) &&
                    p.Email.Contains(searchString, StringComparison.OrdinalIgnoreCase)).ToList();
                break;
            case nameof(Person.DateOfBirth):
                matchingPersons = allPersons.Where(p =>
                    p.DateOfBirth != null &&
                    p.DateOfBirth.Value.ToString("dd MMM yyyy")
                        .Contains(searchString, StringComparison.OrdinalIgnoreCase)).ToList();
                break;
            case nameof(Person.Gender):
                matchingPersons = allPersons.Where(p =>
                    p.Gender != null &&
                    p.Gender.Contains(searchString, StringComparison.OrdinalIgnoreCase)).ToList();
                break;
            case nameof(Person.CountryId):
                matchingPersons = allPersons.Where(p =>
                    p.CountryId != null &&
                    p.CountryId.Value.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase)).ToList();
                break;
            case nameof(Person.Address):
                matchingPersons = allPersons.Where(p =>
                    p.Address != null &&
                    p.Address.Contains(searchString, StringComparison.OrdinalIgnoreCase)).ToList();
                break;
            default:
                matchingPersons = allPersons;
                break;
        }

        return matchingPersons;
    }

    private PersonResponse ConvertPersonToPersonResponse(Person person)
    {
        var personResponse = person.ToPersonResponse();
        personResponse.CountryName = _countriesService.GetCountryByCountryId(person.CountryId)?.CountryName;
        return personResponse;
    }
}