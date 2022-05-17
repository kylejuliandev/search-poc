using Azure.Search.Documents;
using CompaniesFunctions.Company.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Azure;
using System.Text.Json;

namespace CompaniesFunctions.Customer;

public class AzureSearchCompanyFunction
{
    private readonly SearchClient _client;

    public AzureSearchCompanyFunction(IAzureClientFactory<SearchClient> searchFactory)
    {
        _client = searchFactory.CreateClient("CompanySearchClient");
    }

    [Function("azure-search-company")]
    public async Task<SearchCompanyResponse> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequestData req)
    {
        var data = await JsonSerializer.DeserializeAsync<SearchRequestBody>(req.Body);
        if (data is null) return new SearchCompanyResponse(Array.Empty<CompanyDto>());

        var query = new SearchOptions
        {
            Size = data.Size + 1,
            Skip = data.Skip,
            IncludeTotalCount = true,
        };

        var response = await _client.SearchAsync<Search.Data.Company>("*", query);
        var companies = await response.Value.GetResultsAsync()
            .Select(c => new CompanyDto(c.Document.Id, c.Document.Name))
            .ToArrayAsync();

        return new SearchCompanyResponse(companies);
    }
}
