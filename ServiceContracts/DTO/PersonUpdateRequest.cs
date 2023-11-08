using System.ComponentModel.DataAnnotations;
using Entities;
using ServiceContracts.DTO.Enums;

namespace ServiceContracts.DTO;

/// <summary>
///     Acts as a DTO for update a person
/// </summary>
public class PersonUpdateRequest
{
    [Required(ErrorMessage = "Person id must be provided")]
    public Guid PersonId { get; set; }

    [Required(ErrorMessage = "Person name cannot be blank")]
    public string? PersonName { get; set; }

    [Required(ErrorMessage = "Email cannot be blank")]
    [EmailAddress(ErrorMessage = "Please enter a valid email address")]
    public string? Email { get; set; }

    public DateTime? DateOfBirth { get; set; }
    public GenderOptions? Gender { get; set; }
    public Guid? CountryId { get; set; }
    public string? Address { get; set; }
    public bool ReceiveNewsLetters { get; set; }

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