using Entities;

namespace ServiceContracts.DTO;

/// <summary>
///     Represents DTO class that is used as return type of most methods of Person Service
/// </summary>
public class PersonResponse
{
    public Guid PersonId { get; set; }
    public string? PersonName { get; set; }
    public string? Email { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? Gender { get; set; }
    public Guid? CountryId { get; set; }
    public string? CountryName { get; set; }
    public string? Address { get; set; }
    public bool ReceiveNewsLetters { get; set; }
    public double? Age { get; set; }

    /// <summary>
    ///     Compares the current object data with the parameter object
    /// </summary>
    /// <param name="obj">The PersonResponse object to compare</param>
    /// <returns>True or false, indicating whether all person details are matched with the specified parameter object</returns>
    public override bool Equals(object? obj)
    {
        if (obj == null) return false;

        if (obj.GetType() != typeof(PersonResponse)) return false;

        var personToCompare = obj as PersonResponse;
        return PersonId == personToCompare?.PersonId &&
               PersonName == personToCompare.PersonName &&
               Email == personToCompare.Email &&
               DateOfBirth == personToCompare.DateOfBirth &&
               Gender == personToCompare.Gender &&
               CountryId == personToCompare.CountryId &&
               CountryName == personToCompare.CountryName &&
               Address == personToCompare.Address &&
               ReceiveNewsLetters == personToCompare.ReceiveNewsLetters;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}

public static class PersonExtensions
{
    /// <summary>
    ///     An extension method to convert an object of type Person into a PersonResponse object
    /// </summary>
    /// <param name="person">The Person object to convert</param>
    /// <returns>Returns the converted PersonResponse object</returns>
    public static PersonResponse ToPersonResponse(this Person person)
    {
        return new PersonResponse
        {
            PersonId = person.PersonId,
            PersonName = person.PersonName,
            Email = person.Email,
            DateOfBirth = person.DateOfBirth,
            Gender = person.Gender,
            CountryId = person.CountryId,
            Address = person.Address,
            ReceiveNewsLetters = person.ReceiveNewsLetters,
            Age = person.DateOfBirth != null
                ? Math.Round((DateTime.Now - person.DateOfBirth.Value).TotalDays / 365)
                : null
        };
    }
}