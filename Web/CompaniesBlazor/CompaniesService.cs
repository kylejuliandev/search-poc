using System.Net.Http.Json;
using System.Web;

namespace CompaniesBlazor;

public class CompaniesService
{
    private readonly HttpClient _httpClient;

    public CompaniesService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<CustomerResponse?> GetCustomersAsync(string? searchText, IDictionary<string, string> filters, int pageNumber, int pageSize)
    {
        var queryParams = new Dictionary<string, string>();

        if (!string.IsNullOrEmpty(searchText))
        {
            queryParams.Add("searchText", searchText);
        }
        queryParams.Add("pageNumber", pageNumber.ToString());
        queryParams.Add("pageSize", pageSize.ToString());

        foreach (var filter in filters)
        {
            queryParams.Add("filter", $"{filter.Key}|{filter.Value}");
        }

        var queryString = ToQueryString(queryParams);

        var request = new HttpRequestMessage(HttpMethod.Get, $"api/search-customer{queryString}");
        var response = await _httpClient.SendAsync(request);

        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<CustomerResponse>();
        }
        else
        {
            return null;
        }
    }

    public async Task<string[]> GetSuggestedCustomersAsync(string? searchText)
    {
        if (string.IsNullOrEmpty(searchText)) return Array.Empty<string>();

        var queryParams = new Dictionary<string, string>
        {
            { "searchText", searchText }
        };

        var queryString = ToQueryString(queryParams);

        var request = new HttpRequestMessage(HttpMethod.Get, $"api/suggest-customer{queryString}");
        var response = await _httpClient.SendAsync(request);

        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<SuggestCustomerResponse>();
            return result?.SuggestedCustomers.ToArray() ?? Array.Empty<string>();
        }
        else
        {
            return Array.Empty<string>();
        }
    }

    public async Task<CompanyResponse?> GetCompaniesAsync()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "api/search-company");
        var response = await _httpClient.SendAsync(request);

        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<CompanyResponse>();
        }
        else
        {
            return null;
        }
    }

    private static string ToQueryString(IDictionary<string, string> queryParams)
    {
        var mappedQueryParams = queryParams.Select((kvp) => $"{HttpUtility.UrlEncode(kvp.Key)}={HttpUtility.UrlEncode(kvp.Value)}");
        return "?" + string.Join("&", mappedQueryParams);
    }
}

public class CustomerResponse
{
    public IEnumerable<Customer> Customers { get; set; }

    public bool HasMore { get; set; }
}

public class SuggestCustomerResponse
{
    public IEnumerable<string> SuggestedCustomers { get; set; }
}

public class Customer
{
    public string Id { get; set;  }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string EmailAddress { get; set;  }

    public string CompanyId { get; set; }

    public DateTime LatestConnectedOn { get; set;  }

    public DateTime LatestInvitedOn { get; set; }
}

public class CompanyResponse
{
    public IEnumerable<Company> Companies { get; set; }
}

public class Company
{
    public string Id { get; set; }

    public string Name { get; set; }
}