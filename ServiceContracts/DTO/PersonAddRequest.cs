using Entities;
using ServiceContracts.DTO.Enums;

namespace ServiceContracts.DTO;

/// <summary>
///     Acts as a DTO for inserting a new person
/// </summary>
public class PersonAddRequest
{
    public string? PersonName { get; set; }
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