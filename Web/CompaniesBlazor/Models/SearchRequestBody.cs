using System.Text.Json.Serialization;

namespace CompaniesBlazor.Models;

public class SearchRequestBody
{
    [JsonPropertyName("q")]
    public string? SearchText { get; }

    [JsonPropertyName("skip")]
    public int Skip { get; }

    [JsonPropertyName("top")]
    public int Size { get; }

    [JsonPropertyName("filters")]
    public IReadOnlyCollection<SearchRequestFilter> Filters { get; }

    [JsonPropertyName("orderBy")]
    public IReadOnlyCollection<SearchOrderBy> OrderBy { get; }

    public SearchRequestBody(string? searchText, int skip, int size, 
        IReadOnlyCollection<SearchRequestFilter> filters, IReadOnlyCollection<SearchOrderBy> orderBy)
    {
        SearchText = searchText;
        Skip = skip;
        Size = size;
        Filters = filters;
        OrderBy = orderBy;
    }
}
