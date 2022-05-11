using Azure;
using Azure.Search.Documents;
using Azure.Search.Documents.Indexes;
using Bogus;
using Microsoft.Extensions.Configuration;
using Nest;
using Search.Data;
using Search.Data.Loader;
using Spectre.Console;

var config = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .Build();

// Company data
var companyFaker = new Faker<Company>()
    .RuleFor(c => c.Id, f => f.Random.Guid().ToString())
    .RuleFor(c => c.Name, f => f.Company.CompanyName());

var numOrganisations = AnsiConsole.Prompt(
    new TextPrompt<int>("How many Organisations would you like to generate?")
        .DefaultValue(6)
        .Validate(n => n switch
        {
            < 0 => false,
            _ => true
        }));

var companies = companyFaker.Generate(numOrganisations);
var companyIds = companies.Select(c => c.Id);

// Customer data
var customerFaker = new Faker<Customer>()
    .RuleFor(c => c.Id, f => f.Random.Guid().ToString())
    .RuleFor(c => c.FirstName, f => f.Name.FirstName())
    .RuleFor(c => c.FirstNamePartial, (_, c) => c.FirstName)
    .RuleFor(c => c.LastName, f => f.Name.LastName())
    .RuleFor(c => c.LastNamePatial, (_, c) => c.LastName)
    .RuleFor(c => c.EmailAddress, (f, c) => f.Internet.Email(c.FirstName, c.LastName))
    .RuleFor(c => c.CompanyId, f => f.PickRandom(companyIds))
    .RuleFor(c => c.LatestInvitedOn, f => f.Date.Between(DateTime.UtcNow.AddDays(-30), DateTime.UtcNow.AddDays(-1)))
    .RuleFor(c => c.LatestConnectedOn, (f, c) => f.Date.Between(c.LatestInvitedOn.AddMinutes(30), c.LatestInvitedOn.AddDays(1)));

var numCustomers = AnsiConsole.Prompt(
    new TextPrompt<int>("How many Customers would you like to generate?")
        .DefaultValue(100)
        .Validate(n => n switch
        {
            < 0 => false,
            > 100_000 => false,
            _ => true
        }));

var customers = customerFaker.Generate(numCustomers);

// Loaders

if (AnsiConsole.Ask("Do you wish to you Azure Cognitive Search?", false))
{
    var azureSearchOrigin = new Uri(config["SearchSettings:Azure:Origin"]!);
    var azureSearchCredential = new AzureKeyCredential(config["SearchSettings:Azure:ApiKey"]!);

    var indexClient = new SearchIndexClient(azureSearchOrigin, azureSearchCredential);

    var custLoader = new AzureSearchCustomerLoader(customers);
    var custIndex = config["SearchSettings:Azure:Indexes:Customer"]!;
    await custLoader.RunAsync(azureSearchOrigin, azureSearchCredential, custIndex);

    var companyLoader = new AzureSearchCompanyLoader(companies);
    var companyIndex = config["SearchSettings:Azure:Indexes:Company"]!;
    await companyLoader.RunAsync(azureSearchOrigin, azureSearchCredential, companyIndex);
}

if (AnsiConsole.Ask("Do you wish to use Elasticsearch?", false))
{
    var elasticSearchOrigin = new Uri(config["SearchSettings:Elastic:Origin"]!);

    var settings = new ConnectionSettings(elasticSearchOrigin)
        .CertificateFingerprint(config["SearchSettings:Elastic:CertificateFingerprint"]!)
        .BasicAuthentication(config["SearchSettings:Elastic:Username"]!, config["SearchSettings:Elastic:Password"]!);

    var client = new ElasticClient(settings);

    var companyLoader = new ElasticSearchCompanyLoader(companies);
    await companyLoader.RunAsync(client);
}
