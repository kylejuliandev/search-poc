using System.Text.Json.Serialization;

namespace CompaniesFunctions.Company.Models;

public class CompanyDto
{
    [JsonPropertyName("id")]
    public string Id { get; }

    [JsonPropertyName("name")]
    public string Name { get; }

    public CompanyDto(string id, string name)
    {
        Id = id;
        Name = name;
    }
}
