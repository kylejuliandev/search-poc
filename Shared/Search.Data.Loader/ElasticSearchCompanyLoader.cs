using Nest;
using Spectre.Console;

namespace Search.Data.Loader;

internal class ElasticSearchCompanyLoader
{
    private readonly IReadOnlyCollection<Company> _companies;

    public ElasticSearchCompanyLoader(IReadOnlyCollection<Company> companies)
    {
        _companies = companies;
    }

    public async Task RunAsync(ElasticClient client)
    {
        if (AnsiConsole.Ask("Do you want to remove the Company index?", true))
        {
            var exists = await client.Indices.ExistsAsync("company");

            if (exists.Exists)
            {
                AnsiConsole.MarkupLine("[yellow]Removing Company index[/]");
                await client.Indices.DeleteAsync("company");
            }

            AnsiConsole.MarkupLine("[yellow]Recreating Company index[/]");
            await client.Indices.CreateAsync("company", esc =>
                esc.Map<Company>(c =>
                    c.Properties(prop => prop
                        .Keyword(t => t.Name(c => c.Id))
                        .Text(t => t.Name(c => c.Name))
                    )
                ));
        }

        if (AnsiConsole.Ask($"Do you want to upload {_companies.Count} Companies to the Index?", true))
        {
            AnsiConsole.MarkupLine("[yellow]Indexing {0} Companies[/]", _companies.Count);
            await client.IndexManyAsync(_companies, "company");
        }
    }
}
