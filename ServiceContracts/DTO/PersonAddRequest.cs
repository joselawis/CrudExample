using System.ComponentModel.DataAnnotations;
using Entities;
using ServiceContracts.DTO.Enums;

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
    public string? Email { get; init; }

    public DateTime? DateOfBirth { get; init; }
    public GenderOptions? Gender { get; init; }
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
            DateOfBirth = DateOfBirth,
            Gender = Gender.ToString(),
            CountryId = CountryId,
            Address = Address,
            ReceiveNewsLetters = ReceiveNewsLetters
        };
    }
}