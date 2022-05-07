using CompaniesBlazor;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

var companiesOrigin = builder.Configuration["CompaniesOrigin"]!;
builder.Services.AddHttpClient<CompaniesService>(client =>
{
    client.BaseAddress = new Uri(companiesOrigin);
});

await builder.Build().RunAsync();
