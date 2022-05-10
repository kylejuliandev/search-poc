using Azure;
using Azure.Search.Documents.Indexes;
using Azure.Search.Documents.Indexes.Models;
using Spectre.Console;

namespace Search.Data.Loader;

internal class LoaderBase
{
    protected static async Task TryRemoveIndex(SearchIndexClient indexClient, string indexName)
    {
        try
        {
            if (await indexClient.GetIndexAsync(indexName) is not null)
            {
                await indexClient.DeleteIndexAsync(indexName);
            }
        }
        catch (RequestFailedException e) when (e.Status == 404)
        {
            AnsiConsole.MarkupLine("[red]Index does not exist[/]");
        }
    }

    protected static async Task CreateIndex(SearchIndexClient indexClient, string indexName, Type entityType, string suggestorName, string[] suggestorFields)
    {
        var fieldBuilder = new FieldBuilder();
        var searchFields = fieldBuilder.Build(entityType);
        var definition = new SearchIndex(indexName, searchFields);

        var suggester = new SearchSuggester(suggestorName, suggestorFields);
        definition.Suggesters.Add(suggester);

        await indexClient.CreateOrUpdateIndexAsync(definition);
    }
}