using Azure.Search.Documents.Indexes;

namespace Search.Data;

public class Company
{
    public const string SuggestorName = "cp-name";

    [SimpleField(IsKey = true)]
    public string Id { get; set; }

    [SearchableField(IsFilterable = true, IsSortable = true)]
    public string Name { get; set; }
}
