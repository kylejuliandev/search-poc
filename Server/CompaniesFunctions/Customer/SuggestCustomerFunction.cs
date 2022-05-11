using Azure.Search.Documents;
using CompaniesFunctions.Customer.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Azure;
using System.Text.Json;

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
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequestData req)
    {
        var data = await JsonSerializer.DeserializeAsync<SuggestRequestBody>(req.Body);
        if (data is null) return new SuggestCustomerResponse(Array.Empty<string>());

        var query = new SuggestOptions
        {
            Size = 5
        };

        var response = await _client.SuggestAsync<Search.Data.Customer>(data.SearchText, Search.Data.Customer.SuggestorName, query);

        return new SuggestCustomerResponse(response.Value.Results.Select(r => r.Text));
    }
}
