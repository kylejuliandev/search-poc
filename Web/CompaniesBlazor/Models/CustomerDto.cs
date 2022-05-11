using System.Text.Json.Serialization;

namespace CompaniesBlazor.Models;

public class CustomerDto
{
    [JsonPropertyName("id")]
    public string Id { get; init; } = string.Empty;

    [JsonPropertyName("firstName")]
    public string FirstName { get; init; } = string.Empty;

    [JsonPropertyName("lastName")]
    public string LastName { get; init; } = string.Empty;

    [JsonPropertyName("emailAddress")]
    public string EmailAddress { get; init; } = string.Empty;

    [JsonPropertyName("companyId")]
    public string CompanyId { get; init; } = string.Empty;

    [JsonPropertyName("latestConnectedOn")]
    public DateTime LatestConnectedOn { get; init; }

    [JsonPropertyName("latestInvitedOn")]
    public DateTime LatestInvitedOn { get; init; }
}
