using System.Text.Json.Serialization;

namespace CompaniesFunctions.Customer.Models;

public class CustomerDto
{
    [JsonPropertyName("id")]
    public string Id { get; }

    [JsonPropertyName("firstName")]
    public string FirstName { get; }

    [JsonPropertyName("lastName")]
    public string LastName { get; }

    [JsonPropertyName("emailAddress")]
    public string EmailAddress { get; }

    [JsonPropertyName("companyId")]
    public string CompanyId { get; }

    [JsonPropertyName("latestConnectedOn")]
    public DateTime LatestConnectedOn { get; }

    [JsonPropertyName("latestInvitedOn")]
    public DateTime LatestInvitedOn { get; }

    public CustomerDto(string id, string firstName, string lastName, string emailAddress, string companyId,
        DateTime latestConnectedOn, DateTime latestInvitedOn)
    {
        Id = id;
        FirstName = firstName;
        LastName = lastName;
        EmailAddress = emailAddress;
        CompanyId = companyId;
        LatestConnectedOn = latestConnectedOn;
        LatestInvitedOn = latestInvitedOn;
    }
}