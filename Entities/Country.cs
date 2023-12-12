using System.ComponentModel.DataAnnotations;

namespace Entities;

/// <summary>
///     Domain Model for Country
/// </summary>
public class Country
{
    [Key]
    public Guid CountryId { get; set; }

    [StringLength(40)]
    public string? CountryName { get; init; }

    public virtual ICollection<Person>? Persons { get; set; }
}
