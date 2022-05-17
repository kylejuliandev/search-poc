using CompaniesBlazor.Models;
using System.Net.Http.Json;

namespace CompaniesBlazor;

public class CompaniesService
{
    private readonly HttpClient _httpClient;

    public CompaniesService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<SearchCustomerResponse?> GetCustomersAsync(string? searchText, IDictionary<string, string> filters, IDictionary<string, string> orderBy,
        int pageNumber, int pageSize)
    {
        var requestFilters = filters.Select(kvp => new SearchRequestFilter(kvp.Key, kvp.Value)).ToArray();
        var requestOrderBy = orderBy.Select(kvp => new SearchOrderBy(kvp.Key, kvp.Value)).ToArray();
        var requestBody = new SearchRequestBody(searchText, pageNumber * pageSize, pageSize, 
            requestFilters, requestOrderBy);

        var request = new HttpRequestMessage(HttpMethod.Post, "api/azure-search-customer")
        {
            Content = JsonContent.Create(requestBody)
        };

        var response = await _httpClient.SendAsync(request);

        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<SearchCustomerResponse>();
        }
        else
        {
            return null;
        }
    }

    public async Task<string[]> GetSuggestedCustomersAsync(string? searchText)
    {
        if (string.IsNullOrEmpty(searchText)) return Array.Empty<string>();

        var requestBody = new SuggestRequestBody(searchText);
        var request = new HttpRequestMessage(HttpMethod.Post, "api/azure-suggest-customer")
        {
            Content = JsonContent.Create(requestBody)
        };
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

    public async Task<SearchCompanyResponse?> GetCompaniesAsync()
    {
        var requestBody = new SearchRequestBody("8", 0, 10, Array.Empty<SearchRequestFilter>(), Array.Empty<SearchOrderBy>());
        var request = new HttpRequestMessage(HttpMethod.Post, "api/azure-search-company")
        {
            Content = JsonContent.Create(requestBody)
        };
        var response = await _httpClient.SendAsync(request);

        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<SearchCompanyResponse>();
        }
        else
        {
            return null;
        }
    }
}
