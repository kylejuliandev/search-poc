using Azure;
using Azure.Search.Documents.Indexes;
using Azure.Search.Documents.Models;
using Spectre.Console;

namespace Search.Data.Loader;

internal class AzureSearchCustomerLoader : AzureSearchLoaderBase
{
    private readonly IReadOnlyCollection<Customer> _customers;

    public AzureSearchCustomerLoader(IReadOnlyCollection<Customer> customers)
    {
        _customers = customers;
    }

    public async Task RunAsync(Uri origin, AzureKeyCredential credential, string custIndex)
    {
        var indexClient = new SearchIndexClient(origin, credential);

        if (AnsiConsole.Ask("Do you want to remove the Customer index?", true))
        {
            AnsiConsole.MarkupLine("[yellow]Removing the Customer index[/]");
            await TryRemoveIndex(indexClient, custIndex);

            AnsiConsole.MarkupLine("[yellow]Recreating the Customer index[/]");
            await CreateIndex(indexClient, custIndex, typeof(Customer), Customer.SuggestorName, 
                new[] { nameof(Customer.FirstName), nameof(Customer.LastName) });
        }

        if (AnsiConsole.Ask($"Do you want to upload {_customers.Count} Customers to the Index?", true))
        {
            AnsiConsole.MarkupLine("[yellow]Indexing {0} Customer documents[/]", _customers.Count);
            await TryUploadDocuments(indexClient, custIndex);
        }
    }

    private async Task TryUploadDocuments(SearchIndexClient indexClient, string custIndex)
    {
        const int azure_max_entities_to_process = 5000;

        var processedCount = 0;
        var searchClient = indexClient.GetSearchClient(custIndex);

        do
        {
            var entitesToProcess = _customers
                .Skip(processedCount)
                .Take(azure_max_entities_to_process)
                .Select(IndexDocumentsAction.Upload)
                .ToArray();

            processedCount += entitesToProcess.Length;
            AnsiConsole.MarkupLine("[green]Processing batch of {0} Customers, {1} Customers remaining[/]", entitesToProcess.Length,
                _customers.Count - processedCount);

            var batch = IndexDocumentsBatch.Create<Customer>(entitesToProcess);
            
            try
            {
                await searchClient.IndexDocumentsAsync(batch);
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine("{0}", ex.Message);
                AnsiConsole.MarkupLine("[red]Failed to index some of the documents[/]");
            }
        } while (processedCount < _customers.Count);
    }
}
