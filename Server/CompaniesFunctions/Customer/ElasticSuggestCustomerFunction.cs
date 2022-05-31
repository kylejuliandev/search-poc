using CompaniesFunctions.Customer.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Nest;
using System.Net;
using System.Text.Json;

namespace CompaniesFunctions.Customer;

public class ElasticSuggestCustomerFunction
{
    private readonly ElasticClient _client;

    public ElasticSuggestCustomerFunction(ElasticClient elasticClient)
    {
        _client = elasticClient;
    }

    [Function("elastic-suggest-customer")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequestData req)
    {
        var response = req.CreateResponse();

        var data = await JsonSerializer.DeserializeAsync<SuggestRequestBody>(req.Body);
        if (data is null)
        {
            response.StatusCode = HttpStatusCode.BadRequest;
            return response;
        };

        var results = await _client.SearchAsync<Search.Data.Customer>(s => s
            .Index("customer")
            .Query(q => q
                .MultiMatch(m => m
                    .Query(data.SearchText)
                    .Type(TextQueryType.BoolPrefix)
                    .Fields(f => f
                        .Field("firstName")
                        .Field("firstName._2gram")
                        .Field("firstName._3gram")
                        .Field("lastName")
                        .Field("lastName._2gram")
                        .Field("lastName._3gram")
                    )
                )
            )
            .Size(5)
        );

        var suggestedResults = new List<string>();
        foreach (var suggest in results.Documents)
        {
            suggestedResults.Add($"{suggest.FirstName} {suggest.LastName}");
        };

        await response.WriteAsJsonAsync(new SuggestCustomerResponse(suggestedResults));

        return response;
    }
}