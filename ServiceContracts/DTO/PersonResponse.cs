using Entities;
using ServiceContracts.Enums;

namespace ServiceContracts.DTO;

/// <summary>
///     Represents DTO class that is used as return type of most methods of Person Service
/// </summary>
public class PersonResponse
{
    public Guid PersonId { get; set; }
    public string? PersonName { get; init; }
    public string? Email { get; init; }
    public DateTime? DateOfBirth { get; init; }
    public string? Gender { get; init; }
    public Guid? CountryId { get; init; }
    public string? CountryName { get; set; }
    public string? Address { get; init; }
    public bool ReceiveNewsLetters { get; init; }
    public double? Age { get; init; }

    /// <summary>
    ///     Compares the current object data with the parameter object
    /// </summary>
    /// <param name="obj">The PersonResponse object to compare</param>
    /// <returns>True or false, indicating whether all person details are matched with the specified parameter object</returns>
    public override bool Equals(object? obj)
    {
        if (obj == null)
            return false;

        if (obj.GetType() != typeof(PersonResponse))
            return false;

        var personToCompare = obj as PersonResponse;
        return PersonId == personToCompare?.PersonId
               && PersonName == personToCompare.PersonName
               && Email == personToCompare.Email
               && DateOfBirth == personToCompare.DateOfBirth
               && Gender == personToCompare.Gender
               && CountryId == personToCompare.CountryId
               && CountryName == personToCompare.CountryName
               && Address == personToCompare.Address
               && ReceiveNewsLetters == personToCompare.ReceiveNewsLetters;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public override string ToString()
    {
        return $"PersonId: {PersonId}, PersonName: {PersonName}, Email: {Email}, DateOfBirth: {DateOfBirth?
            .ToString("dd MMM yyyy")}, Gender: {Gender}, CountryId: {CountryId}, CountryName: {
            CountryName}, Address: {Address}, ReceiveNewsLetters: {ReceiveNewsLetters}, Age: {Age}";
    }

    public PersonUpdateRequest ToPersonUpdateRequest()
    {
        return new PersonUpdateRequest
        {
            PersonId = PersonId,
            PersonName = PersonName,
            Email = Email,
            DateOfBirth = DateOfBirth,
            Gender =
                Gender != null
                    ? (GenderOptions)Enum.Parse(typeof(GenderOptions), Gender, true)
                    : null,
            CountryId = CountryId,
            Address = Address,
            ReceiveNewsLetters = ReceiveNewsLetters
        };
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
            Age =
                person.DateOfBirth != null
                    ? Math.Round(
                        (DateTime.Now - person.DateOfBirth.Value).TotalDays / 365,
                        MidpointRounding.ToZero
                    )
                    : null,
            CountryName = person.Country?.CountryName
        };
    }
}
