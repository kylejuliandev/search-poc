using System.Text.Json.Serialization;

namespace CompaniesBlazor.Models;

public class SearchOrderBy
{
    [JsonPropertyName("field")]
    public string Field { get; }

    [JsonPropertyName("direction")]
    public string Direction { get; }

    public SearchOrderBy(string field, string direction)
    {
        Field = field;
        Direction = direction;
    }
}