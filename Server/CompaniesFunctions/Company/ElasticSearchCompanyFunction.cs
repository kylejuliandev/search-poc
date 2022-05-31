using CompaniesFunctions.Company.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CompaniesFunctions.Company;

public class ElasticSearchCompanyFunction
{
    private readonly ElasticClient _client;

    public ElasticSearchCompanyFunction(ElasticClient elasticClient)
    {
        _client = elasticClient;
    }

    [Function("elastic-search-company")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequestData req)
    {
        var response = req.CreateResponse();

        var data = await JsonSerializer.DeserializeAsync<SearchRequestBody>(req.Body);
        if (data is null)
        {
            response.StatusCode = HttpStatusCode.BadRequest;
            return response;
        };

        var results = await _client.SearchAsync<Search.Data.Company>(s => s
            .Index("company")
            .Query(q => q
                .Match(m => m
                    .Query("")
                )
            )
            .Size(5)
        );

        var companies = results.Documents.Select(c => new CompanyDto(c.Id, c.Name));

        await response.WriteAsJsonAsync(new SearchCompanyResponse(companies));

        return response;
    }
}