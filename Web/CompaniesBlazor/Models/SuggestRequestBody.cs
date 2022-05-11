using System.Text.Json.Serialization;

namespace CompaniesBlazor.Models;

public class SuggestRequestBody
{
    [JsonPropertyName("q")]
    public string SearchString { get; }

    public SuggestRequestBody(string searchString)
    {
        SearchString = searchString;
    }
}
