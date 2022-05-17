using System.Text.Json.Serialization;

namespace CompaniesFunctions;

public class SearchRequestBody
{
    [JsonPropertyName("q")]
    public string? SearchText { get; init; }

    [JsonPropertyName("skip")]
    public int Skip { get; init; }

    [JsonPropertyName("top")]
    public int Size { get; init; }

    [JsonPropertyName("filters")]
    public IReadOnlyCollection<SearchRequestFilter> Filters { get; init; } = Array.Empty<SearchRequestFilter>();

    [JsonPropertyName("orderBy")]
    public IReadOnlyCollection<SearchOrderBy> OrderBy { get; init; } = Array.Empty<SearchOrderBy>();
}
