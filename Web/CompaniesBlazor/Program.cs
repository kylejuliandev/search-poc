using CompaniesBlazor;
using CompaniesRpc;
using Grpc.Net.Client;
using Grpc.Net.Client.Web;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

var backendOrigin = builder.Configuration["BackendOrigin"]!;

builder.Services.AddSingleton<Greeter.GreeterClient>((_) =>
{
    var channel = GrpcChannel.ForAddress(
        backendOrigin,
        new GrpcChannelOptions
        {
            HttpHandler = new GrpcWebHandler(new HttpClientHandler())
        });

    return new Greeter.GreeterClient(channel);
});

await builder.Build().RunAsync();
