using Azure;
using Azure.Search.Documents;
using Azure.Search.Documents.Indexes;
using Azure.Search.Documents.Indexes.Models;
using Azure.Search.Documents.Models;
using Bogus;
using Microsoft.Extensions.Configuration;
using Search.Data;
using Spectre.Console;

var config = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .Build();

var numOrganisations = AnsiConsole.Prompt(
    new TextPrompt<int>("How many Organisations would you like to generate?")
        .Validate(n => n switch
        {
            < 0 => false,
            _ => true
        }));

var randomizer = new Randomizer();
var organisationIds = Enumerable.Range(0, numOrganisations).Select(i => randomizer.Guid().ToString());

var faker = new Faker<Customer>()
    .RuleFor(c => c.Id, f => f.Random.Guid().ToString())
    .RuleFor(c => c.FirstName, f => f.Name.FirstName())
    .RuleFor(c => c.LastName, f => f.Name.LastName())
    .RuleFor(c => c.EmailAddress, (f, c) => f.Internet.Email(c.FirstName, c.LastName))
    .RuleFor(c => c.OrganisationId, f => f.PickRandom(organisationIds))
    .RuleFor(c => c.LatestInvitedOn, f => f.Date.Between(DateTime.UtcNow.AddDays(-30), DateTime.UtcNow.AddDays(-1)))
    .RuleFor(c => c.LatestConnectedOn, (f, c) => f.Date.Between(c.LatestInvitedOn.AddMinutes(30), c.LatestInvitedOn.AddDays(1)));

var numCustomers = AnsiConsole.Prompt(
    new TextPrompt<int>("How many Customers would you like to generate?")
        .Validate(n => n switch
        {
            < 0 => false,
            > 10000 => false,
            _ => true
        }));

var customers = faker.Generate(numCustomers);

var origin = new Uri(config["SearchSettings:Origin"]!);
var custIndex = config["SearchSettings:Indexes:Customer"]!;
var credential = new AzureKeyCredential(config["SearchSettings:ApiKey"]!);

var indexClient = new SearchIndexClient(origin, credential);

try
{
    if (indexClient.GetIndex(custIndex) is not null)
    {
        AnsiConsole.MarkupLine("[red]Removing index[/]");
        indexClient.DeleteIndex(custIndex);
    }
}
catch (RequestFailedException e) when (e.Status == 404)
{
    AnsiConsole.MarkupLine("[yellow]Index does not exist[/]");
}

var fieldBuilder = new FieldBuilder();
var searchFields = fieldBuilder.Build(typeof(Customer));

AnsiConsole.MarkupLine("[green]Creating index[/]");
var definition = new SearchIndex(custIndex, searchFields);
await indexClient.CreateOrUpdateIndexAsync(definition);

var batch = IndexDocumentsBatch.Create<Customer>();
foreach (var customer in customers)
{
    AnsiConsole.MarkupLine("Adding batch item for Customer [yellow]{0}[/]", customer.Id);
    batch.Actions.Add(IndexDocumentsAction.Upload(customer));
}

var searchClient = indexClient.GetSearchClient(custIndex);

AnsiConsole.MarkupLine("[yellow]Indexing documents[/]");
try
{
    await searchClient.IndexDocumentsAsync(batch);
}
catch(Exception)
{
    AnsiConsole.MarkupLine("[red]Failed to index some of the documents[/]");
    return;
}