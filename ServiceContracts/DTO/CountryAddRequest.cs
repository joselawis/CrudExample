using Entities;

namespace ServiceContracts.DTO;

/// <summary>
///     DTO Class for adding a new country
/// </summary>
public class CountryAddRequest
{
    public string? CountryName { get; init; }

    public Country ToCountry()
    {
        return new Country { CountryName = CountryName };
    }
}
