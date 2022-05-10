using Azure.Search.Documents;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Azure;

namespace CompaniesFunctions.Customer;

public class SearchCompanyFunction
{
    private readonly SearchClient _client;

    public SearchCompanyFunction(IAzureClientFactory<SearchClient> searchFactory)
    {
        _client = searchFactory.CreateClient("CompanySearchClient");
    }

    [Function("search-company")]
    public async Task<SearchCompanyResponse> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequestData req)
    {
        var query = new SearchOptions
        {
            Size = 10,
            Skip = 0,
            QueryType = Azure.Search.Documents.Models.SearchQueryType.Simple
        };

        var response = await _client.SearchAsync<Search.Data.Company>("*", query);
        var companies = await response.Value.GetResultsAsync()
            .Select(c => new Company(c.Document.Id, c.Document.Name))
            .ToArrayAsync();

        return new SearchCompanyResponse(companies);
    }
}
