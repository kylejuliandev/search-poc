using System.Text.Json.Serialization;

namespace CompaniesBlazor.Models;

public class SuggestCustomerResponse
{
    [JsonPropertyName("suggestedCustomers")]
    public IReadOnlyCollection<string> SuggestedCustomers { get; init; } = Array.Empty<string>();
}
