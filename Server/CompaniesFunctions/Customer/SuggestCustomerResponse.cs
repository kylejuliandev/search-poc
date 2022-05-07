namespace CompaniesFunctions.Customer;

public class SuggestCustomerResponse
{
    public IEnumerable<string> SuggestedCustomers { get; }

    public SuggestCustomerResponse(IEnumerable<string> suggestedCustomers)
    {
        SuggestedCustomers = suggestedCustomers;
    }
}
