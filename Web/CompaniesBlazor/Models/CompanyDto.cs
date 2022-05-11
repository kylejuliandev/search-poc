using System.Text.Json.Serialization;

namespace CompaniesBlazor.Models;

public class CompanyDto
{
    [JsonPropertyName("id")]
    public string Id { get; init; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;
}