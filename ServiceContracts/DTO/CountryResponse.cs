using Entities;

namespace ServiceContracts.DTO;

/// <summary>
///     DTO Class that is used as return type for most of CountryService methods
/// </summary>
public class CountryResponse
{
    public Guid CountryId { get; init; }
    public string? CountryName { get; init; }

    public override bool Equals(object? obj)
    {
        if (obj == null)
            return false;

        if (obj.GetType() != typeof(CountryResponse))
            return false;

        var countryToCompare = obj as CountryResponse;
        return countryToCompare != null
               && CountryId == countryToCompare.CountryId
               && CountryName == countryToCompare.CountryName;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}

public static class CountryExtensions
{
    public static CountryResponse ToCountryResponse(this Country country)
    {
        return new CountryResponse
        {
            CountryId = country.CountryId,
            CountryName = country.CountryName
        };
    }
}
