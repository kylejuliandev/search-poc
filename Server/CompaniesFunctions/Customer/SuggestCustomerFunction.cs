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

        var response = await _client.SuggestAsync<Search.Data.Customer>(searchText, Search.Data.Customer.SuggestorName, query);

        var result = new SuggestCustomerResponse(response.Value.Results.Select(r => r.Text));

        return result;
    }
}
