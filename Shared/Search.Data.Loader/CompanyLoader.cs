using Azure;
using Azure.Search.Documents.Indexes;
using Azure.Search.Documents.Models;
using Spectre.Console;

namespace Search.Data.Loader;

internal class CompanyLoader : LoaderBase
{
    private readonly IReadOnlyCollection<Company> _companies;

    public CompanyLoader(IReadOnlyCollection<Company> companies)
    {
        _companies = companies;
    }

    public async Task RunAsync(Uri origin, AzureKeyCredential credential, string companyIndex)
    {
        var indexClient = new SearchIndexClient(origin, credential);

        if (AnsiConsole.Ask("Do you want to remove the Company index?", true))
        {
            AnsiConsole.MarkupLine("[yellow]Removing the Company index[/]");
            await TryRemoveIndex(indexClient, companyIndex);

            AnsiConsole.MarkupLine("[yellow]Recreating the Company index[/]");
            await CreateIndex(indexClient, companyIndex, typeof(Company), Company.SuggestorName, 
                new[] { nameof(Company.Name) });
        }

        if (AnsiConsole.Ask($"Do you want to upload {_companies.Count} Companies to the Index?", true))
        {
            AnsiConsole.MarkupLine("[yellow]Indexing {0} Company documents[/]", _companies.Count);
            await TryUploadDocuments(indexClient, companyIndex);
        }
    }

    private async Task TryUploadDocuments(SearchIndexClient indexClient, string custIndex)
    {
        const int azure_max_entities_to_process = 5000;

        var processedCount = 0;
        var searchClient = indexClient.GetSearchClient(custIndex);

        do
        {
            var entitesToProcess = _companies
                .Skip(processedCount)
                .Take(azure_max_entities_to_process)
                .Select(IndexDocumentsAction.Upload)
                .ToArray();

            processedCount += entitesToProcess.Length;
            AnsiConsole.MarkupLine("[green]Processing batch of {0} Companies, {1} Companies remaining[/]", entitesToProcess.Length,
                _companies.Count - processedCount);

            var batch = IndexDocumentsBatch.Create<Company>(entitesToProcess);

            try
            {
                await searchClient.IndexDocumentsAsync(batch);
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine("{0}", ex.Message);
                AnsiConsole.MarkupLine("[red]Failed to index some of the documents[/]");
            }
        } while (processedCount < _companies.Count);
    }
}
