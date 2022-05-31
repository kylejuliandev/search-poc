using Nest;
using Spectre.Console;

namespace Search.Data.Loader;

internal class ElasticSearchCustomerLoader
{
    private readonly IReadOnlyCollection<Customer> _customers;

    public ElasticSearchCustomerLoader(IReadOnlyCollection<Customer> customers)
    {
        _customers = customers;
    }

    public async Task RunAsync(ElasticClient client)
    {
        if (AnsiConsole.Ask("Do you want to remove the Customer index?", true))
        {
            var indexExistsResponse = await client.Indices.ExistsAsync("customer");

            if (indexExistsResponse.Exists)
            {
                AnsiConsole.MarkupLine("[yellow]Removing Customer index[/]");
                await client.Indices.DeleteAsync("customer");
            }

            AnsiConsole.MarkupLine("[yellow]Recreating Customer index[/]");
            await client.Indices.CreateAsync("customer", esc =>
                esc.Map<Customer>(c => c
                    .Properties(prop => prop
                        .Keyword(t => t.Name(c => c.Id))
                        .SearchAsYouType(t => t.Name(c => c.FirstName))
                        .SearchAsYouType(t => t.Name(c => c.LastName))
                        .Keyword(t => t.Name(c => c.EmailAddress))
                        .Keyword(t => t.Name(c => c.CompanyId))
                        .Date(t => t.Name(c => c.LatestInvitedOn))
                        .Date(t => t.Name(c => c.LatestConnectedOn))
                    )
                ));
        }

        if (AnsiConsole.Ask($"Do you want to upload {_customers.Count} Customers to the Index?", true))
        {
            AnsiConsole.MarkupLine("[yellow]Indexing {0} Customers[/]", _customers.Count);
            await client.IndexManyAsync(_customers, "customer");
        }
    }
}