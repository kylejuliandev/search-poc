using System.Text.Json.Serialization;

namespace CompaniesBlazor.Models;

public class SearchCompanyResponse
{
    [JsonPropertyName("companies")]
    public IReadOnlyCollection<CompanyDto> Companies { get; init; } = Array.Empty<CompanyDto>();
}
