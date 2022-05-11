using System.Text.Json.Serialization;

namespace CompaniesBlazor.Models;

public class SearchRequestFilter
{
    [JsonPropertyName("field")]
    public string? Field { get; }

    [JsonPropertyName("value")]
    public string? Value { get; }

    public SearchRequestFilter(string? field, string? value)
    {
        Field = field;
        Value = value;
    }
}
