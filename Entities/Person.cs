namespace Entities;

/// <summary>
///     Person domain model class
/// </summary>
public class Person
{
    public Guid PersonId { get; set; }
    public string? PersonName { get; init; }
    public string? Email { get; init; }
    public DateTime? DateOfBirth { get; init; }
    public string? Gender { get; init; }
    public Guid? CountryId { get; init; }
    public string? Address { get; init; }
    public bool ReceiveNewsLetters { get; init; }
}