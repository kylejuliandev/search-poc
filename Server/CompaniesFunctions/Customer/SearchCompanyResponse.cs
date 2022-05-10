namespace CompaniesFunctions.Customer;

public class SearchCompanyResponse
{
    public IEnumerable<Company> Companies { get; }

    public SearchCompanyResponse(IEnumerable<Company> companies)
    {
        Companies = companies;
    }
}

public class Company
{
    public string Id { get; }

    public string Name { get; }

    public Company(string id, string name)
    {
        Id = id;
        Name = name;
    }
}
