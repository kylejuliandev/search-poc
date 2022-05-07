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
            Size = request.PageSettings.PageSize + 1,
            Skip = request.PageSettings.PageNumber * request.PageSettings.PageSize
        };

        var searchText = request.SearchText;
        searchText = string.IsNullOrEmpty(searchText) ? "*" : searchText;

        var response = await _client.SearchAsync<Customer>(searchText, query);
        var reply = new CustomerReply();

        var results = await response.Value.GetResultsAsync().ToArrayAsync();

        foreach (var cust in results.Take(request.PageSettings.PageSize))
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

        reply.TotalCount = results.Length;
        reply.HasMore = results.Length > request.PageSettings.PageSize;

        return reply;
    }
}
