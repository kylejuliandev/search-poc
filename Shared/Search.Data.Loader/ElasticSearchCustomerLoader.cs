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
                        .Keyword(t => t.Name(nameof(Customer.Id)))
                        .Text(t => t.Name(nameof(Customer.FirstName)))
                        .Text(t => t.Name(nameof(Customer.LastName)))
                        .Keyword(t => t.Name(nameof(Customer.EmailAddress)))
                        .Keyword(t => t.Name(nameof(Customer.CompanyId)))
                        .Date(t => t.Name(nameof(Customer.LatestInvitedOn)))
                        .Date(t => t.Name(nameof(Customer.LatestConnectedOn)))
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
