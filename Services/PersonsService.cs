using Entities;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Services.Helpers;

namespace Services;

public class PersonsService : IPersonsService
{
    private readonly ICountriesService _countriesService = new CountriesService();
    private readonly List<Person> _persons;

    public PersonsService(bool initialize = true)
    {
        _persons = new List<Person>();
        if (initialize)
            _persons.AddRange(new List<Person>
            {
                new()
                {
                    PersonId = Guid.Parse("81AFB72D-8F61-4434-B3EC-4F5839209F6A"), PersonName = "Edward",
                    Email = "egisburn0@answers.com", DateOfBirth = DateTime.Parse("12/02/2019"), Gender = "Male",
                    CountryId = Guid.Parse("055338DF-6A83-4F8B-9FB5-06C87D9A7AEA"), Address = "0536 Karstens Plaza",
                    ReceiveNewsLetters = true
                },
                new()
                {
                    PersonId = Guid.Parse("999AEF88-DB49-4C3B-9DE1-42D32CDA9449"), PersonName = "Ganny",
                    Email = "gboldra1@nasa.gov", DateOfBirth = DateTime.Parse("12/11/1981"), Gender = "Male",
                    CountryId = Guid.Parse("8E7E9725-E2F8-460A-B5A1-8027EA8AE966"), Address = "6679 Old Shore Court",
                    ReceiveNewsLetters = false
                },
                new()
                {
                    PersonId = Guid.Parse("1F45ECFB-A012-4F18-97C7-5ADA6E4653AB"), PersonName = "Jennifer",
                    Email = "jennings2@answers.com", DateOfBirth = DateTime.Parse("12/02/2019"), Gender = "Male",
                    CountryId = Guid.Parse("2C9DDA3C-F729-40C4-A32E-18D072CE23EA"), Address = "0536 Karstens Plaza",
                    ReceiveNewsLetters = false
                },
                new()
                {
                    PersonId = Guid.Parse("FD0D618A-4670-48B4-BE12-15CCCAEFAA64"), PersonName = "Clerkclaude",
                    Email = "cwensley2@homestead.com", DateOfBirth = DateTime.Parse("23/03/1986"), Gender = "Male",
                    CountryId = Guid.Parse("055338DF-6A83-4F8B-9FB5-06C87D9A7AEA"), Address = "0 Gale Lane",
                    ReceiveNewsLetters = false
                },
                new()
                {
                    PersonId = Guid.Parse("2932C0F7-A535-4280-BDF8-70EDB220D1E1"), PersonName = "Creighton",
                    Email = "cosburn3@elegantthemes.com", DateOfBirth = DateTime.Parse("30/05/1982"), Gender = "Male",
                    CountryId = Guid.Parse("8E7E9725-E2F8-460A-B5A1-8027EA8AE966"), Address = "68265 Kim Place",
                    ReceiveNewsLetters = true
                },
                new()
                {
                    PersonId = Guid.Parse("7C8CE909-78D7-4D2A-8A83-A5A94B9A7B15"), PersonName = "Shamus",
                    Email = "sfoxen4@wp.com", DateOfBirth = DateTime.Parse("13/09/1978"), Gender = "Male",
                    CountryId = Guid.Parse("743CC7A2-EDC1-4757-85C2-6502EE650984"), Address = "033 Mcbride Alley",
                    ReceiveNewsLetters = true
                },
                new()
                {
                    PersonId = Guid.Parse("8DC5FC41-843B-47AF-9F6F-6424126A11EF"), PersonName = "Aurilia",
                    Email = "atellenbrok5@creativecommons.org", DateOfBirth = DateTime.Parse("22/01/2015"),
                    Gender = "Female", CountryId = Guid.Parse("B24DC622-A001-4FDC-B7CA-C7E96E999C26"),
                    Address = "5463 7th Lane", ReceiveNewsLetters = true
                },
                new()
                {
                    PersonId = Guid.Parse("38E6C5CF-5C65-4255-92E8-F3706B6A55B3"), PersonName = "Inga",
                    Email = "idresse6@yellowbook.com", DateOfBirth = DateTime.Parse("09/12/1991"), Gender = "Female",
                    CountryId = Guid.Parse("055338DF-6A83-4F8B-9FB5-06C87D9A7AEA"), Address = "76049 Springs Point",
                    ReceiveNewsLetters = true
                },
                new()
                {
                    PersonId = Guid.Parse("AEE2DB2E-C637-4D48-B267-C2935ADB37AA"), PersonName = "Kris",
                    Email = "klicence7@exblog.jp", DateOfBirth = DateTime.Parse("09/05/2006"), Gender = "Female",
                    CountryId = Guid.Parse("743CC7A2-EDC1-4757-85C2-6502EE650984"),
                    Address = "65509 Little Fleur Place", ReceiveNewsLetters = true
                },
                new()
                {
                    PersonId = Guid.Parse("9B0B997B-2575-46D0-B385-CE050DF94A6C"), PersonName = "Teirtza",
                    Email = "tharrower8@naver.com", DateOfBirth = DateTime.Parse("20/03/2022"), Gender = "Female",
                    CountryId = Guid.Parse("8E7E9725-E2F8-460A-B5A1-8027EA8AE966"), Address = "045 Dorton Plaza",
                    ReceiveNewsLetters = true
                },
                new()
                {
                    PersonId = Guid.Parse("244B8138-50DC-4A64-B3B6-E429BC2456F1"), PersonName = "Gavin",
                    Email = "gborsnall9@simplemachines.org", DateOfBirth = DateTime.Parse("05/07/1977"),
                    Gender = "Male", CountryId = Guid.Parse("B24DC622-A001-4FDC-B7CA-C7E96E999C26"),
                    Address = "60 Ronald Regan Lane", ReceiveNewsLetters = true
                }
            });
    }

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
                    p.Gender != null && p.Gender.Equals(searchString, StringComparison.OrdinalIgnoreCase))
                .ToList(),
            nameof(PersonResponse.CountryId) => allPersons.Where(p =>
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
        if (personUpdateRequest == null) throw new ArgumentNullException(nameof(personUpdateRequest));

        ValidationHelper.ModelValidation(personUpdateRequest);

        var matchingPerson = _persons.FirstOrDefault(p => p.PersonId == personUpdateRequest.PersonId);
        if (matchingPerson == null) throw new ArgumentException("Person not found");

        matchingPerson.PersonName = personUpdateRequest.PersonName;
        matchingPerson.Email = personUpdateRequest.Email;
        matchingPerson.DateOfBirth = personUpdateRequest.DateOfBirth;
        matchingPerson.Gender = personUpdateRequest.Gender?.ToString();
        matchingPerson.CountryId = personUpdateRequest.CountryId;
        matchingPerson.Address = personUpdateRequest.Address;
        matchingPerson.ReceiveNewsLetters = personUpdateRequest.ReceiveNewsLetters;

        return ConvertPersonToPersonResponse(matchingPerson);
    }

    public bool DeletePerson(Guid? personId)
    {
        if (personId == null) throw new ArgumentNullException(nameof(personId));
        var removed = _persons.RemoveAll(p => p.PersonId == personId);
        return removed > 0;
    }

    private static List<PersonResponse> ToSortedList(IEnumerable<PersonResponse> allPersons,
        Func<PersonResponse, object?> keySelector, SortOrderOptions sortOrder)
    {
        return (sortOrder.Equals(SortOrderOptions.Asc)
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