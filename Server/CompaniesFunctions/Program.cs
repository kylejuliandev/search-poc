using Azure;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Nest;

var host = new HostBuilder()
	.ConfigureFunctionsWorkerDefaults()
	.ConfigureAppConfiguration(builder =>
    {
		if (Environment.GetEnvironmentVariable("AZURE_FUNCTIONS_ENVIRONMENT") == "Development")
		{
			builder.AddUserSecrets<Program>();
		}
	})
	.ConfigureServices((builder, s) =>
    {
		s.AddAzureClients(azBuilder =>
		{
			var origin = new Uri(builder.Configuration["SearchSettings:Azure:Origin"]!);
			var credential = new AzureKeyCredential(builder.Configuration["SearchSettings:Azure:ApiKey"]!);

			var custIndex = builder.Configuration["SearchSettings:Azure:Indexes:Customer"]!;
			azBuilder.AddSearchClient(origin, custIndex, credential).WithName("CustomerSearchClient");

			var companyIndex = builder.Configuration["SearchSettings:Azure:Indexes:Company"]!;
			azBuilder.AddSearchClient(origin, companyIndex, credential).WithName("CompanySearchClient");
		});

		s.AddTransient(c =>
		{
			var elasticSearchOrigin = new Uri(builder.Configuration["SearchSettings:Elastic:Origin"]!);
			var certificateFingerprint = builder.Configuration["SearchSettings:Elastic:CertificateFingerprint"]!;
			var elasticUser = builder.Configuration["SearchSettings:Elastic:Username"]!;
			var elasticPassword = builder.Configuration["SearchSettings:Elastic:Password"]!;

			var settings = new ConnectionSettings(elasticSearchOrigin)
				.CertificateFingerprint(certificateFingerprint)
				.BasicAuthentication(elasticUser, elasticPassword);

			return new ElasticClient(settings);
		});
    })
	.Build();

await host.RunAsync();