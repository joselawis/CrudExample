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

    private PersonResponse ConvertPersonToPersonResponse(Person person)
    {
        var personResponse = person.ToPersonResponse();
        personResponse.CountryName = _countriesService.GetCountryByCountryId(person.CountryId)?.CountryName;
        return personResponse;
    }
}