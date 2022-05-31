using CompaniesFunctions.Customer.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Nest;
using System.Net;
using System.Text.Json;

namespace CompaniesFunctions.Customer;

public class ElasticSearchCustomerFunction
{
    private readonly ElasticClient _client;

    public ElasticSearchCustomerFunction(ElasticClient elasticClient)
    {
        _client = elasticClient;
    }

    [Function("elastic-search-customer")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequestData req)
    {
        var response = req.CreateResponse();

        var data = await JsonSerializer.DeserializeAsync<SearchRequestBody>(req.Body);
        if (data is null)
        {
            response.StatusCode = HttpStatusCode.BadRequest;
            return response;
        };

        var filters = BuildFilters(data.Filters);
        var sort = BuildSortBy(data.OrderBy);

        var results = await _client.SearchAsync<Search.Data.Customer>(s => s
            .Index("customer")
            .From(data.Skip)
            .Take(data.Size + 1)
            .Query(q => q
                .Bool(b => b
                    .Must(m => m
                        .MultiMatch(mm => mm
                            .Query(data.SearchText)
                            .Type(TextQueryType.BoolPrefix)
                            .Fields(f => f
                                .Field("firstName")
                                .Field("firstName._2gram")
                                .Field("firstName._3gram")
                                .Field("lastName")
                                .Field("lastName._2gram")
                                .Field("lastName._3gram")
                                .Field(ff => ff.EmailAddress)
                            )
                        )
                    )
                    .Filter(pf => pf
                        .Bool(b => b
                            .Filter(filters)
                        )
                    )
                )
            )
            .Sort(s => sort));

        var customers = results.Documents.Take(data.Size).Select(ToCustomer);
        await response.WriteAsJsonAsync(new SearchCustomerResponse(customers, results.Documents.Count > data.Size, results.Total));

        return response;
    }

    private static QueryContainer[] BuildFilters(IReadOnlyCollection<SearchRequestFilter> searchFilters)
    {
        var filters = new QueryContainer[searchFilters.Count];
        for (var i = 0; i < searchFilters.Count; i++)
        {
            var filter = searchFilters.ElementAt(i);
            var desc = new QueryContainerDescriptor<Search.Data.Customer>();
            var container = desc
                .Match(m => m
                    .Field(new Field(filter.Field))
                    .Query(filter.Value)
            );

            filters[i] = container;
        }

        return filters;
    }

    private static SortDescriptor<Search.Data.Customer> BuildSortBy(IReadOnlyCollection<SearchOrderBy> orderBy)
    {
        var sortDesc = new SortDescriptor<Search.Data.Customer>();
        foreach (var order in orderBy)
        {
            sortDesc.Field(new Field(order.Field), order.Direction == "asc"
                ? SortOrder.Ascending
                : SortOrder.Descending);
        }

        return sortDesc;
    }

    private static CustomerDto ToCustomer(Search.Data.Customer customer) => new(
        customer.Id,
        customer.FirstName,
        customer.LastName,
        customer.EmailAddress,
        customer.CompanyId,
        customer.LatestConnectedOn,
        customer.LatestInvitedOn);
}