using System.Text.Json.Serialization;

namespace CompaniesFunctions;

public class SearchOrderBy
{
    [JsonPropertyName("field")]
    public string? Field { get; init; }

    [JsonPropertyName("direction")]
    public string? Direction { get; init; }
}
