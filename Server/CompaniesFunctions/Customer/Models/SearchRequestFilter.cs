using System.Text.Json.Serialization;

namespace CompaniesFunctions.Customer.Models;

public class SearchRequestFilter
{
    [JsonPropertyName("field")]
    public string? Field { get; init; }

    [JsonPropertyName("value")]
    public string? Value { get; init; }
}