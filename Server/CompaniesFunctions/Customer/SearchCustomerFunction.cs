using Azure.Search.Documents;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Azure;

namespace CompaniesFunctions.Customer;

public class SearchCustomerFunction
{
    private readonly SearchClient _client;

    public SearchCustomerFunction(IAzureClientFactory<SearchClient> searchFactory)
    {
        _client = searchFactory.CreateClient("CustomerSearchClient");
    }

    [Function("search-customer")]
    public async Task<SearchCustomerResponse> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequestData req)
	{
        var queryDict = System.Web.HttpUtility.ParseQueryString(req.Url.Query);
        var pageNumber = Convert.ToInt32(queryDict.Get("pageNumber") ?? "0");
        var pageSize = Convert.ToInt32(queryDict.Get("pageSize") ?? "0");
        var searchText = queryDict.Get("searchText") ?? string.Empty;

        var query = new SearchOptions
        {
            Size = pageSize + 1,
            Skip = pageNumber * pageSize,
            QueryType = Azure.Search.Documents.Models.SearchQueryType.Simple
        };

        searchText = string.IsNullOrEmpty(searchText) ? "*" : searchText;

        var response = await _client.SearchAsync<Search.Data.Customer>(searchText, query);
        var customers = await response.Value.GetResultsAsync().ToArrayAsync();

        var result = new SearchCustomerResponse(
            customers.Take(pageSize).Select(c => ToCustomer(c.Document)), 
            customers.Length > pageSize);

        return result;
    }

    private static Customer ToCustomer(Search.Data.Customer customer) => new(
        customer.Id,
        customer.FirstName,
        customer.LastName,
        customer.EmailAddress,
        customer.OrganisationId,
        customer.LatestConnectedOn,
        customer.LatestInvitedOn);
}
