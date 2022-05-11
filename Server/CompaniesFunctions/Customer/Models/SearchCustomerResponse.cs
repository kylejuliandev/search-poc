using System.Text.Json.Serialization;

namespace CompaniesFunctions.Customer.Models;

public class SearchCustomerResponse
{
    [JsonPropertyName("customers")]
    public IEnumerable<CustomerDto> Customers { get; }

    [JsonPropertyName("hasMore")]
    public bool HasMore { get; }

    [JsonPropertyName("totalCount")]
    public long TotalCount { get; }

    public SearchCustomerResponse(IEnumerable<CustomerDto> customers, bool hasMore, long totalCount)
    {
        Customers = customers;
        HasMore = hasMore;
        TotalCount = totalCount;
    }
}
