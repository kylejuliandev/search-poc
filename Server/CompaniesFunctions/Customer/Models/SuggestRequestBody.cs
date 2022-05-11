using System.Text.Json.Serialization;

namespace CompaniesFunctions.Customer.Models;

public class SuggestRequestBody
{
    [JsonPropertyName("q")]
    public string? SearchText { get; init; }
}
