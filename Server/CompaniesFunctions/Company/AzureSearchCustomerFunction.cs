using Azure.Search.Documents;
using CompaniesFunctions.Customer.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Azure;
using System.Text.Json;

namespace CompaniesFunctions.Company;

public class AzureSearchCustomerFunction
{
    private readonly SearchClient _client;

    public AzureSearchCustomerFunction(IAzureClientFactory<SearchClient> searchFactory)
    {
        _client = searchFactory.CreateClient("CustomerSearchClient");
    }

    [Function("azure-search-customer")]
    public async Task<SearchCustomerResponse> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequestData req)
    {
        var data = await JsonSerializer.DeserializeAsync<SearchRequestBody>(req.Body);
        if (data is null) return new SearchCustomerResponse(Array.Empty<CustomerDto>(), false, 0);

        var query = new SearchOptions
        {
            Size = data.Size + 1,
            Skip = data.Skip,
            IncludeTotalCount = true,
            Filter = CreateFilterExpression(data.Filters)
        };

        foreach (var orderBy in data.OrderBy)
        {
            var field = orderBy.Field;
            var direction = orderBy.Direction;

            if (!string.IsNullOrEmpty(field) && !string.IsNullOrEmpty(direction))
            {
                query.OrderBy.Add($"{field} {direction}");
            }
        }

        var response = await _client.SearchAsync<Search.Data.Customer>(data.SearchText ?? "*", query);
        var customers = await response.Value.GetResultsAsync()
            .Select(c => ToCustomer(c.Document))
            .ToArrayAsync();

        return new SearchCustomerResponse(customers.Take(data.Size), customers.Length > data.Size, response.Value.TotalCount ?? 0);
    }

    public static string? CreateFilterExpression(IReadOnlyCollection<SearchRequestFilter> filters)
    {
        if (filters is null || filters.Count <= 0) return null;

        var filterExpressions = new List<string>();

        var companyFilters = filters.Where(f => f.Field is not null and "CompanyId");
        var companyFilterValues = companyFilters.Where(v => v.Value is not null).Select(f => f.Value);

        if (companyFilterValues.Any())
        {
            var filterStr = string.Join(",", companyFilterValues);
            filterExpressions.Add($"search.in({nameof(CustomerDto.CompanyId)}, '{filterStr}', ',')");
        }

        var result = string.Join(" and ", filterExpressions);
        Console.WriteLine(result);

        return result;
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