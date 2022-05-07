using Azure;
using CompaniesRpc.Services;
using Microsoft.Extensions.Azure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpc();
builder.Services.AddCors(o => o.AddPolicy("AllowAll", builder =>
{
    builder.AllowAnyOrigin()
           .AllowAnyMethod()
           .AllowAnyHeader()
           .WithExposedHeaders("Grpc-Status", "Grpc-Message", "Grpc-Encoding", "Grpc-Accept-Encoding");
}));

builder.Services.AddAzureClients(azBuilder =>
{
    var origin = new Uri(builder.Configuration["SearchSettings:Origin"]!);
    var custIndex = builder.Configuration["SearchSettings:Indexes:Customer"]!;
    var credential = new AzureKeyCredential(builder.Configuration["SearchSettings:ApiKey"]!);

    azBuilder.AddSearchClient(origin, custIndex, credential).WithName("CustomerSearchClient");
});

var app = builder.Build();

app.UseGrpcWeb();
app.UseCors();

// Configure the HTTP request pipeline.
app.MapGrpcService<GreeterService>().EnableGrpcWeb()
    .RequireCors("AllowAll");

app.Run();
