using Azure.Search.Documents;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Extensions.Azure;
using Search.Data;

namespace CompaniesRpc.Services;
public class GreeterService : Greeter.GreeterBase
{
    private readonly ILogger<GreeterService> _logger;
    private readonly SearchClient _client;

    public GreeterService(ILogger<GreeterService> logger, IAzureClientFactory<SearchClient> searchFactory)
    {
        _logger = logger;
        _client = searchFactory.CreateClient("CustomerSearchClient");
    }

    public async override Task<CustomerReply> GetCustomers(CustomerRequest request, ServerCallContext context)
    {
        var query = new SearchOptions
        {
            IncludeTotalCount = true
        };

        //if (orderDirection is not null && orderByProperties is not null)
        //{
        //    foreach (var prop in orderByProperties)
        //    {
        //        var direction = orderDirection == OrderDirection.Ascending ? "asc" : "desc";
        //        var orderBy = $"{prop} {direction}";

        //        query.OrderBy.Add(orderBy);
        //    }
        //}

        var searchText = request.SearchText;
        searchText = string.IsNullOrEmpty(searchText) ? "*" : searchText;

        var response = await _client.SearchAsync<Customer>(searchText, query);
        var reply = new CustomerReply();

        await foreach (var cust in response.Value.GetResultsAsync())
        {
            reply.Customers.Add(new CustomerRpc
            {
                Id = cust.Document.Id,
                FirstName = cust.Document.FirstName,
                LastName = cust.Document.LastName,
                EmailAddress = cust.Document.EmailAddress,
                OrganisationId = cust.Document.OrganisationId,
                LatestConnectedOn = cust.Document.LatestConnectedOn.ToUniversalTime().ToTimestamp(),
                LastedInvitedOn = cust.Document.LatestInvitedOn.ToUniversalTime().ToTimestamp()
            });
        }

        return reply;
    }
}
