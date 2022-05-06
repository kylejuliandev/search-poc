using CompaniesBlazor;
using CompaniesRpc;
using Grpc.Net.Client.Web;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

var backendOrigin = builder.Configuration["BackendOrigin"]!;

builder.Services
    .AddGrpcClient<Greeter.GreeterClient>(options =>
    {
        options.Address = new Uri(backendOrigin);
    })
    .ConfigurePrimaryHttpMessageHandler(() => new GrpcWebHandler(new HttpClientHandler()));

await builder.Build().RunAsync();
