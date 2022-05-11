using System.Text.Json.Serialization;

namespace CompaniesFunctions.Customer.Models;

public class SuggestCustomerResponse
{
    [JsonPropertyName("suggestedCustomers")]
    public IEnumerable<string> SuggestedCustomers { get; }

    public SuggestCustomerResponse(IEnumerable<string> suggestedCustomers)
    {
        SuggestedCustomers = suggestedCustomers;
    }
}
