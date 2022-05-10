using Azure.Search.Documents;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Azure;
using System.Text;

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
        var filters = queryDict.Get("filter");

        var query = new SearchOptions
        {
            Size = pageSize + 1,
            Skip = pageNumber * pageSize,
            QueryType = Azure.Search.Documents.Models.SearchQueryType.Simple
        };

        if (filters is not null)
        {
            var filterBuilder = new List<string>();
            foreach (var filterKvp in filters.Split(','))
            {
                var filterMap = filterKvp.Split('|');
                var filterKey = filterMap[0];
                var filterValue = filterMap[1];

                var property = filterKey switch
                {
                    nameof(Customer.CompanyId) => nameof(Customer.CompanyId),
                    _ => null
                };

                if (property is not null)
                {
                    filterBuilder.Add($"{property} eq '{filterValue}'");
                }
            }

            var filterString = string.Join(" and ", filterBuilder);
            query.Filter = filterString;
        }

        searchText = string.IsNullOrEmpty(searchText) ? "*" : searchText;

        var response = await _client.SearchAsync<Search.Data.Customer>(searchText, query);
        var customers = await response.Value.GetResultsAsync()
            .Take(pageSize)
            .Select(c => ToCustomer(c.Document))
            .ToArrayAsync();

        return new SearchCustomerResponse(
            customers,
            customers.Length > pageSize);
    }

    private static Customer ToCustomer(Search.Data.Customer customer) => new(
        customer.Id,
        customer.FirstName,
        customer.LastName,
        customer.EmailAddress,
        customer.CompanyId,
        customer.LatestConnectedOn,
        customer.LatestInvitedOn);
}