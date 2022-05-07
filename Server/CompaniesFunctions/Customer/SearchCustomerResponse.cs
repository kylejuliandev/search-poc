namespace CompaniesFunctions.Customer;

public class SearchCustomerResponse
{
    public IEnumerable<Customer> Customers { get; }

    public bool HasMore { get; }

    public SearchCustomerResponse(IEnumerable<Customer> customers, bool hasMore)
    {
        Customers = customers;
        HasMore = hasMore;
    }
}

public class Customer
{
    public string Id { get; }

    public string FirstName { get; }

    public string LastName { get; }

    public string EmailAddress { get; }

    public string OrganisationId { get; }

    public DateTime LatestConnectedOn { get; }

    public DateTime LatestInvitedOn { get; }

    public Customer(string id, string firstName, string lastName, string emailAddress, string organisationId, 
        DateTime latestConnectedOn, DateTime latestInvitedOn)
    {
        Id = id;
        FirstName = firstName;
        LastName = lastName;
        EmailAddress = emailAddress;
        OrganisationId = organisationId;
        LatestConnectedOn = latestConnectedOn;
        LatestInvitedOn = latestInvitedOn;
    }
}