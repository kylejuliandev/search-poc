using System.Text.Json.Serialization;

namespace CompaniesBlazor.Models;

public class SearchCustomerResponse
{
    [JsonPropertyName("customers")]
    public IEnumerable<CustomerDto> Customers { get; set; } = Array.Empty<CustomerDto>();

    [JsonPropertyName("hasMore")]
    public bool HasMore { get; set; }

    [JsonPropertyName("totalCount")]
    public long TotalCount { get; set; }
}
