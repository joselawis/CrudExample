using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities;

/// <summary>
///     Person domain model class
/// </summary>
public class Person
{
    [Key]
    public Guid PersonId { get; set; }

    [StringLength(40)]
    public string? PersonName { get; set; }

    [StringLength(40)]
    public string? Email { get; set; }

    [Column(TypeName = "date")]
    public DateTime? DateOfBirth { get; set; }

    [StringLength(6)]
    public string? Gender { get; set; }

    public Guid? CountryId { get; set; }

    [StringLength(200)]
    public string? Address { get; set; }

    public bool ReceiveNewsLetters { get; set; }

    public string? Tin { get; set; }

    [ForeignKey("CountryId")]
    public Country? Country { get; set; }
}
