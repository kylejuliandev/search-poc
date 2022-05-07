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

    public async Task<CustomerResponse?> GetCustomerAsync(string? searchText, int pageNumber, int pageSize)
    {
        var queryParams = new Dictionary<string, string>();

        if (!string.IsNullOrEmpty(searchText))
        {
            queryParams.Add("searchText", searchText);
        }
        queryParams.Add("pageNumber", pageNumber.ToString());
        queryParams.Add("pageSize", pageSize.ToString());

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

public class Customer
{
    public string Id { get; set;  }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string EmailAddress { get; set;  }

    public string OrganisationId { get; set; }

    public DateTime LatestConnectedOn { get; set;  }

    public DateTime LatestInvitedOn { get; set; }
}