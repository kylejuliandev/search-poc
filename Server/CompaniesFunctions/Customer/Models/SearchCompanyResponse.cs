using System.Text.Json.Serialization;

namespace CompaniesFunctions.Customer.Models;

public class SearchCompanyResponse
{
    [JsonPropertyName("companies")]
    public IEnumerable<CompanyDto> Companies { get; }

    public SearchCompanyResponse(IEnumerable<CompanyDto> companies)
    {
        Companies = companies;
    }
}
