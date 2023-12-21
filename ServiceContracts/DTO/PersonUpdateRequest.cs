using System.ComponentModel.DataAnnotations;
using Entities;
using ServiceContracts.Enums;

namespace ServiceContracts.DTO;

/// <summary>
///     Acts as a DTO for update a person
/// </summary>
public class PersonUpdateRequest
{
    [Required(ErrorMessage = "Person id must be provided")]
    public Guid PersonId { get; init; }

    [Required(ErrorMessage = "Person name cannot be blank")]
    public string? PersonName { get; set; }

    [Required(ErrorMessage = "Email cannot be blank")]
    [EmailAddress(ErrorMessage = "Please enter a valid email address")]
    public string? Email { get; set; }

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
            PersonId = PersonId,
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
