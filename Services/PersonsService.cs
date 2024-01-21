using System.Drawing;
using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using RepositoryContracts;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Services.Helpers;

namespace Services;

public class PersonsService : IPersonsService
{
    private readonly ILogger<PersonsService> _logger;
    private readonly IPersonsRepository _personsRepository;

    public PersonsService(IPersonsRepository personsRepository, ILogger<PersonsService> logger)
    {
        _personsRepository = personsRepository;
        _logger = logger;
    }

    public async Task<PersonResponse> AddPerson(PersonAddRequest? personAddRequest)
    {
        if (personAddRequest == null)
            throw new ArgumentNullException(nameof(personAddRequest));

        ValidationHelper.ModelValidation(personAddRequest);

        var person = personAddRequest.ToPerson();
        person.PersonId = Guid.NewGuid();

        await _personsRepository.AddPerson(person);

        return person.ToPersonResponse();
    }

    public async Task<List<PersonResponse>> GetAllPersons()
    {
        var persons = await _personsRepository.GetAllPersons();

        return persons.Select(person => person.ToPersonResponse()).ToList();
    }

    public async Task<PersonResponse?> GetPersonByPersonId(Guid? personId)
    {
        if (personId == null)
            return null;
        var person = await _personsRepository.GetPersonByPersonId(personId.Value);
        return person?.ToPersonResponse();
    }

    public async Task<List<PersonResponse>> GetFilteredPersons(
        string searchBy,
        string? searchString
    )
    {
        _logger.LogInformation("GetFilteredPersons of PersonsService");

        searchString ??= "";
        var persons = searchBy switch
        {
            nameof(PersonResponse.PersonName)
                => await _personsRepository.GetFilteredPersons(
                    p =>
                        p.PersonName != null
                        && p.PersonName.ToLower().Contains(searchString.ToLower())
                ),
            nameof(PersonResponse.Email)
                => await _personsRepository.GetFilteredPersons(
                    p => p.Email != null && p.Email.ToLower().Contains(searchString.ToLower())
                ),
            nameof(PersonResponse.DateOfBirth)
                => await _personsRepository.GetFilteredPersons(
                    p =>
                        p.DateOfBirth != null
                        && p.DateOfBirth.Value.ToString("dd MMM yyyy")
                            .ToLower()
                            .Contains(searchString.ToLower())
                ),
            nameof(PersonResponse.Gender)
                => await _personsRepository.GetFilteredPersons(
                    p => p.Gender != null && p.Gender.ToLower().Equals(searchString.ToLower())
                ),
            nameof(PersonResponse.CountryId)
                => await _personsRepository.GetFilteredPersons(
                    p =>
                        p.Country != null
                        && p.Country.CountryName != null
                        && p.Country.CountryName.ToString()
                            .ToLower()
                            .Contains(searchString.ToLower())
                ),
            nameof(PersonResponse.Address)
                => await _personsRepository.GetFilteredPersons(
                    p => p.Address != null && p.Address.ToLower().Contains(searchString.ToLower())
                ),
            _ => await _personsRepository.GetAllPersons()
        };

        return persons.Select(person => person.ToPersonResponse()).ToList();
    }

    public Task<List<PersonResponse>> GetSortedPersons(
        List<PersonResponse> allPersons,
        string sortBy,
        SortOrderOptions sortOrder
    )
    {
        _logger.LogInformation("GetSortedPersons of PersonsService");

        if (string.IsNullOrEmpty(sortBy))
            return Task.FromResult(allPersons);

        var sortedPersons = sortBy switch
        {
            nameof(PersonResponse.PersonName)
                => ToSortedList(allPersons, p => p.PersonName, sortOrder),
            nameof(PersonResponse.Email) => ToSortedList(allPersons, p => p.Email, sortOrder),
            nameof(PersonResponse.DateOfBirth)
                => ToSortedList(allPersons, p => p.DateOfBirth, sortOrder),
            nameof(PersonResponse.Age) => ToSortedList(allPersons, p => p.Age, sortOrder),
            nameof(PersonResponse.Gender) => ToSortedList(allPersons, p => p.Gender, sortOrder),
            nameof(PersonResponse.CountryId)
                => ToSortedList(allPersons, p => p.CountryName, sortOrder),
            nameof(PersonResponse.Address) => ToSortedList(allPersons, p => p.Address, sortOrder),
            nameof(PersonResponse.ReceiveNewsLetters)
                => ToSortedList(allPersons, p => p.ReceiveNewsLetters, sortOrder),
            _ => allPersons
        };

        return Task.FromResult(sortedPersons);
    }

    public async Task<PersonResponse> UpdatePerson(PersonUpdateRequest? personUpdateRequest)
    {
        if (personUpdateRequest == null)
            throw new ArgumentNullException(nameof(personUpdateRequest));

        ValidationHelper.ModelValidation(personUpdateRequest);

        var matchingPerson = await _personsRepository.GetPersonByPersonId(
            personUpdateRequest.PersonId
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

        await _personsRepository.UpdatePerson(matchingPerson);

        return matchingPerson.ToPersonResponse();
    }

    public async Task<bool> DeletePerson(Guid? personId)
    {
        if (personId == null)
            throw new ArgumentNullException(nameof(personId));

        var person = await _personsRepository.GetPersonByPersonId(personId.Value);
        if (person == null)
            return false;

        return await _personsRepository.DeletePersonByPersonId(personId.Value);
    }

    public async Task<MemoryStream> GetPersonsCsv()
    {
        var memoryStream = new MemoryStream();
        var streamWriter = new StreamWriter(memoryStream);

        var csvConfiguration = new CsvConfiguration(CultureInfo.InvariantCulture);
        var csvWriter = new CsvWriter(streamWriter, csvConfiguration);

        csvWriter.WriteField(nameof(PersonResponse.PersonName));
        csvWriter.WriteField(nameof(PersonResponse.Email));
        csvWriter.WriteField(nameof(PersonResponse.DateOfBirth));
        csvWriter.WriteField(nameof(PersonResponse.Age));
        csvWriter.WriteField(nameof(PersonResponse.Gender));
        csvWriter.WriteField(nameof(PersonResponse.CountryName));
        csvWriter.WriteField(nameof(PersonResponse.Address));
        csvWriter.WriteField(nameof(PersonResponse.ReceiveNewsLetters));
        await csvWriter.NextRecordAsync();

        var persons = await GetAllPersons();

        foreach (var person in persons)
        {
            csvWriter.WriteField(person.PersonName);
            csvWriter.WriteField(person.Email);
            csvWriter.WriteField(
                person.DateOfBirth.HasValue ? person.DateOfBirth.Value.ToString("yyyy-MM-dd") : ""
            );
            csvWriter.WriteField(person.Age);
            csvWriter.WriteField(person.Gender);
            csvWriter.WriteField(person.CountryName);
            csvWriter.WriteField(person.Address);
            csvWriter.WriteField(person.ReceiveNewsLetters);

            await csvWriter.NextRecordAsync();
            await csvWriter.FlushAsync();
        }

        memoryStream.Position = 0;
        return memoryStream;
    }

    public async Task<MemoryStream> GetPersonsExcel()
    {
        var memoryStream = new MemoryStream();
        using var excelPackage = new ExcelPackage(memoryStream);
        var worksheet = excelPackage.Workbook.Worksheets.Add("PersonsSheet");
        worksheet.Cells["A1"].Value = "Person Name";
        worksheet.Cells["B1"].Value = "Email";
        worksheet.Cells["C1"].Value = "Date of Birth";
        worksheet.Cells["D1"].Value = "Age";
        worksheet.Cells["E1"].Value = "Gender";
        worksheet.Cells["F1"].Value = "Country";
        worksheet.Cells["G1"].Value = "Address";
        worksheet.Cells["H1"].Value = "Receive News letters";

        using var headerCells = worksheet.Cells["A1:H1"];
        headerCells.Style.Fill.PatternType = ExcelFillStyle.Solid;
        headerCells.Style.Fill.BackgroundColor.SetColor(Color.LightGray);
        headerCells.Style.Font.Bold = true;

        var row = 2;
        var persons = await GetAllPersons();

        foreach (var person in persons)
        {
            worksheet.Cells[row, 1].Value = person.PersonName;
            worksheet.Cells[row, 2].Value = person.Email;
            worksheet.Cells[row, 3].Value = person.DateOfBirth.HasValue
                ? person.DateOfBirth.Value.ToString("yyyy-MM-dd")
                : "";
            worksheet.Cells[row, 4].Value = person.Age;
            worksheet.Cells[row, 5].Value = person.Gender;
            worksheet.Cells[row, 6].Value = person.CountryName;
            worksheet.Cells[row, 7].Value = person.Address;
            worksheet.Cells[row, 8].Value = person.ReceiveNewsLetters;

            row++;
        }

        worksheet.Cells[$"A1:H{row}"].AutoFitColumns();
        await excelPackage.SaveAsync();

        memoryStream.Position = 0;
        return memoryStream;
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
