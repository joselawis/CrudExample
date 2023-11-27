using System.ComponentModel.DataAnnotations;
using Entities;
using ServiceContracts.Enums;

namespace ServiceContracts.DTO;

/// <summary>
///     Acts as a DTO for inserting a new person
/// </summary>
public class PersonAddRequest
{
    [Required(ErrorMessage = "Person name cannot be blank")]
    public string? PersonName { get; init; }

    [Required(ErrorMessage = "Email cannot be blank")]
    [EmailAddress(ErrorMessage = "Please enter a valid email address")]
    [DataType(DataType.EmailAddress)]
    public string? Email { get; init; }

    [DataType(DataType.Date)] public DateTime? DateOfBirth { get; init; }

    [Required(ErrorMessage = "Please select gender of the person")]
    public GenderOptions? Gender { get; init; }

    [Required(ErrorMessage = "Please select a country")]
    public Guid? CountryId { get; init; }

    public string? Address { get; init; }
    public bool ReceiveNewsLetters { get; init; }

    /// <summary>
    ///     Converts the current object of PersonAddRequest to a Person object
    /// </summary>
    /// <returns>A object of type Person</returns>
    public Person ToPerson()
    {
        return new Person
        {
            PersonName = PersonName,
            Email = Email,
            DateOfBirth = DateOfBirth?.SetKindUtc(),
            Gender = Gender.ToString(),
            CountryId = CountryId,
            Address = Address,
            ReceiveNewsLetters = ReceiveNewsLetters
        };
    }
}