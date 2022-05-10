using Azure;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

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
			var origin = new Uri(builder.Configuration["SearchSettings:Origin"]!);
			var credential = new AzureKeyCredential(builder.Configuration["SearchSettings:ApiKey"]!);

			var custIndex = builder.Configuration["SearchSettings:Indexes:Customer"]!;
			azBuilder.AddSearchClient(origin, custIndex, credential).WithName("CustomerSearchClient");

			var companyIndex = builder.Configuration["SearchSettings:Indexes:Company"]!;
			azBuilder.AddSearchClient(origin, companyIndex, credential).WithName("CompanySearchClient");
		});
	})
	.Build();

await host.RunAsync();