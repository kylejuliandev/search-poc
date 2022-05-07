using Azure.Search.Documents;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Azure;

namespace CompaniesFunctions.Customer;

public class SuggestCustomerFunction
{
    private readonly SearchClient _client;

    public SuggestCustomerFunction(IAzureClientFactory<SearchClient> searchFactory)
    {
        _client = searchFactory.CreateClient("CustomerSearchClient");
    }

    [Function("suggest-customer")]
    public async Task<SuggestCustomerResponse> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequestData req)
    {
        var queryDict = System.Web.HttpUtility.ParseQueryString(req.Url.Query);
        var searchText = queryDict.Get("searchText") ?? string.Empty;

        var query = new SuggestOptions
        {
            Size = 5
        };

        searchText = string.IsNullOrEmpty(searchText) ? "*" : searchText;

        var response = await _client.SuggestAsync<Search.Data.Customer>(searchText, "sg-names", query);
        var customers = response.Value.Results.GroupBy(r => r.Text).Select(r => r.First().Text); // Remove duplicates

        var result = new SuggestCustomerResponse(customers);

        return result;
    }
}
