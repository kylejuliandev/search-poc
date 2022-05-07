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
			var custIndex = builder.Configuration["SearchSettings:Indexes:Customer"]!;
			var credential = new AzureKeyCredential(builder.Configuration["SearchSettings:ApiKey"]!);

			azBuilder.AddSearchClient(origin, custIndex, credential).WithName("CustomerSearchClient");
		});
	})
	.Build();

await host.RunAsync();