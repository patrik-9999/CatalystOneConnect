using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace APIs;

public class CatalystOrgs
{
    public List<Organization>? organizations { get; set; }
}

public class Organization
{
    public string? guid { get; set; }
    public string? organizationId { get; set; }
    public string? createdOn { get; set; }
    public string? lastModified { get; set; }
    public string? isActive { get; set; }
    public FieldOrg? field { get; set; }
}

public class FieldOrg
{
    [JsonPropertyName("0")]
    public _0_org? _0 { get; set; }
    [JsonPropertyName("400")]
    public _400_org? _400 { get; set; }
    [JsonPropertyName("401")]
    public _401_org? _401 { get; set; }
}

public class _0_org
{
    public string? name { get; set; }
    public string? lastModified { get; set; }
    public string? type { get; set; }
    public DataOrg? data { get; set; }
}

public class DataOrg
{
    public string? value { get; set; }
    public string? guid { get; set; }
    public string? organizationId { get; set; }

}

public class _400_org
{
    public string? name { get; set; }
    public string? lastModified { get; set; }
    public string? type { get; set; }
    public DataOrg data { get; set; }
}

public class _401_org
{
    public string? name { get; set; }
    public string? lastModified { get; set; }
    public string? type { get; set; }
    public DataOrg? data { get; set; }
}
