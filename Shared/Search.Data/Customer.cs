using Azure.Search.Documents.Indexes;
using Azure.Search.Documents.Indexes.Models;

namespace Search.Data;

public class Customer
{
    public const string SuggestorName = "sg-name";

    [SimpleField(IsKey = true)]
    public string Id { get; set; }

    [SearchableField(IsFilterable = true, IsSortable = true)]
    public string FirstName { get; set; }

    [SearchableField(AnalyzerName = "lucence-custom-analyzer")]
    public string FirstNamePartial { get; set; }

    [SearchableField(IsFilterable = true, IsSortable = true)]
    public string LastName { get; set; }

    [SearchableField(AnalyzerName = "lucence-custom-analyzer")]
    public string LastNamePatial { get; set; }

    [SearchableField(IsFilterable = true, IsSortable = true)]
    public string EmailAddress { get; set; }

    [SimpleField(IsFilterable = true)]
    public string CompanyId { get; set; }

    [SimpleField(IsSortable = true)]
    public DateTime LatestConnectedOn { get; set; }

    [SimpleField(IsSortable = true)]
    public DateTime LatestInvitedOn { get; set; }
}