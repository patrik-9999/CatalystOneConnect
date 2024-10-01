using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

#pragma warning disable IDE1006  // Namnkonvention för variabler, vill ha samma namn på varibeln som i dokumentationen från Catalyst One

namespace APIs;


[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
public sealed class SqlNameAttribute : Attribute
{
    public string Name { get; }
    public SqlNameAttribute(string name)
    {
        Name = name;
    }
}
public class CatalystEmployees
{
    public List<Employee?>? employees { get; set; }
    
    public static void CreateSql(string fileName)
    {
        var sb = new StringBuilder();
        foreach(var property in typeof(Employee).GetProperties())
        {
            //Console.WriteLine(property.Name);
            //var sqlNameAttribute = Attribute.GetCustomAttribute(property, typeof(SqlNameAttribute));
            //var attrs = property.PropertyType.GetCustomAttributes();
            var sqlNameAttribute = (SqlNameAttribute?)Attribute.GetCustomAttribute(property, typeof(SqlNameAttribute));
            if (sqlNameAttribute is not null)
            {
                Console.WriteLine($"{property.Name} {sqlNameAttribute.Name}");
            }
            if(property.Name=="field")
            {
               foreach(var field in property.PropertyType.GetProperties())
                {
                    var subSqlNameAttribute = (SqlNameAttribute?)Attribute.GetCustomAttribute(field, typeof(SqlNameAttribute));
                    if (subSqlNameAttribute is not null)
                    {
                        var subJsonNameAttribute = (JsonPropertyNameAttribute?)Attribute.GetCustomAttribute(field, typeof(JsonPropertyNameAttribute));
                        if (subJsonNameAttribute is not null)
                        {
                            var dataProp = field.PropertyType.GetProperties().FirstOrDefault(p => p.Name == "data");
                            if (dataProp is not null)
                            {
                                Console.WriteLine($"  {subJsonNameAttribute.Name} {subSqlNameAttribute.Name} {dataProp.PropertyType.Name}");
                                if (dataProp.PropertyType.Name.StartsWith("List") || dataProp.PropertyType.IsArray)
                                {
                                    Console.WriteLine($"  IsCollection");
                                }

                                //var typeName = dataProp.Name;
                                //if (dataProp.PropertyType.IsGenericType)
                                //{
                                //    typeName = typeName.Substring(0, typeName.IndexOf('`'));
                                //    var parameterTypeNames = dataProp.PropertyType.GetGenericArguments(); //.Select type.GetGenericArguments().Select(GetTypeName);
                                //    //typeName = $"{typeName}<{string.Join(", ", parameterTypeNames)}>";
                                //}

                                //if (dataProp.PropertyType.IsGenericType)
                                //{
                                //    Console.WriteLine($"  {subJsonNameAttribute.Name} {subSqlNameAttribute.Name} {dataProp.PropertyType.Name} {dataProp.PropertyType.GetGenericArguments().FirstOrDefault().Name}");
                                //}
                                //else
                                //{
                                //    Console.WriteLine($"  {subJsonNameAttribute.Name} {subSqlNameAttribute.Name} {dataProp.PropertyType.Name}");
                                //}

                                //Console.WriteLine($"  {dataProp.PropertyType.IsArray}");
                                //if (dataProp.PropertyType.GetType())
                                //{
                                //    Console.WriteLine($"  IsArray");
                                //}

                            }
                            //var valueProp = dataProp?.PropertyType.GetProperties().FirstOrDefault(p => p.Name == "value");
                            //if(valueProp is not null)
                            //    Console.WriteLine($"  {subJsonNameAttribute.Name} {subSqlNameAttribute.Name} {valueProp.PropertyType.Name}");
                        }
                        else
                        {
                            Console.WriteLine($"  {field.Name} {subSqlNameAttribute.Name}");
                        }
                    }
                }
            }
            //foreach (var attr in attrs)
            //    Console.WriteLine($"{property.Name}: {attr.GetType().Name}");
            //if (property.PropertyType == typeof(string))
            //    continue;
            //foreach(var subProperty in property.PropertyType.GetProperties())
            //{
            //    Console.WriteLine("  " + subProperty.Name);
            //}
        }
    }
}

public class Employee
{
    [SqlName("guid")]
    public string? guid { get; set; }
    [SqlName("profileGUID")]
    public string? profileGUID { get; set; }
    public string? name { get; set; }
    public string? username { get; set; }
    public string? employeeId { get; set; }
    public string? profileId { get; set; }
    [SqlName("createdOn")]
    public string? createdOn { get; set; }
    [SqlName("lastModified")]
    public string? lastModified { get; set; }
    public string? hasPlannedChange { get; set; }
    public Field? field { get; set; }
}

public class Field
{
    [SqlName("EmployeeID")]
    [JsonPropertyName("0")]
    public _0? _0 { get; set; }         // "name": "Employee ID"
    [SqlName("Name")]
    [JsonPropertyName("1")]
    public _1? _1 { get; set; }         // "name": "Name"
    [SqlName("FirstName")]
    [JsonPropertyName("2")]
    public _2? _2 { get; set; }         // "name": "First name"

    [SqlName("LastName")]
    [JsonPropertyName("3")]
    public _3? _3 { get; set; }         // "name": "Last name"

    [SqlName("Email")]
    [JsonPropertyName("7")]
    public _7? _7 { get; set; }         // "name": "E-mail"

    [SqlName("Organisation")]
    [JsonPropertyName("8")]
    public _8? _8 { get; set; }         // "name": "Organisation"

    [SqlName("Manager")]
    [JsonPropertyName("9")]
    public _9? _9 { get; set; }         // "name": "Manager"

    [SqlName("AdditionalManagers")]
    [JsonPropertyName("10")]
    public _10? _10 { get; set; }       // "name": "Additional managers"

    [SqlName("JobLocation")]
    [JsonPropertyName("14")]
    public _14? _14 { get; set; }       // "name": "Job location",

    [SqlName("JobRole")]
    [JsonPropertyName("15")]
    public _15? _15 { get; set; }       // "name": "Job role"

    [SqlName("Gender")]
    [JsonPropertyName("19")]
    public _19? _19 { get; set; }       // "name": "Gender"

    [SqlName("DateOfBirth")]
    [JsonPropertyName("21")]
    public _21? _21 { get; set; }       // "name": "Date of birth"

    [SqlName("NationalIdentifier")]
    [JsonPropertyName("22")]
    public _22? _22 { get; set; }       // "name": "National identifier"

    [SqlName("DepartmentManager")]
    [JsonPropertyName("32")]
    public _32? _32 { get; set; }       // "name": "Department manager"

    [SqlName("DateOfHiring")]
    [JsonPropertyName("37")]
    public _37? _37 { get; set; }       // "name": "Date of hiring"

    [SqlName("DateOfTermination")]
    [JsonPropertyName("38")]
    public _38? _38 { get; set; }       // "name": "Date of termination"

    [SqlName("PercentageOfFTE")]
    [JsonPropertyName("40")]
    public _40? _40 { get; set; }       // "name": "Percentage of FTE"

    [SqlName("PrimaryProfile")]
    [JsonPropertyName("45")]
    public _45? _45 { get; set; }       // "name": "Primary profile",

    [SqlName("ProfileID")]
    [JsonPropertyName("47")]            
    public _47? _47 { get; set; }       // "name": "Profile ID"

    [SqlName("EmploymentStatus")]
    [JsonPropertyName("49")]
    public _49? _49 { get; set; }       // "name": "Employment status"

    [SqlName("TypeOfPay")]
    [JsonPropertyName("50")]
    public _50? _50 { get; set; }       // "name": "Type of pay"

    [SqlName("Username")]
    [JsonPropertyName("101")]
    public _101? _101 { get; set; }     // "name": "Username"

    [SqlName("UserAccountStatus")]
    [JsonPropertyName("103")]           
    public _103? _103 { get; set; }         // "name": "User account status"

    [SqlName("LegalEntity")]
    [JsonPropertyName("1002")]
    public _1002? _1002 { get; set; }       // "name": "Legal entity"

    [SqlName("CompanyMobilePhone")]
    [JsonPropertyName("1003")]
    public _1003? _1003 { get; set; }       // "name": "Company mobile phone"

    [SqlName("PrivateEmail")]
    [JsonPropertyName("1005")]
    public _1005? _1005 { get; set; }       // "name": "Private e-mail"

    [SqlName("ReasonForLeaving")]
    [JsonPropertyName("1028")]
    public _1028? _1028 { get; set; }       // "name": "Reason for leaving"

    [SqlName("ActualLastWorkingDay")]
    [JsonPropertyName("1029")]
    public _1029? _1029 { get; set; }       // "name": "Actual last working day"

    [SqlName("ContractSigned")]
    [JsonPropertyName("1033")]
    public _1033? _1033 { get; set; }       // "name": "Contract signed"

    [SqlName("ITPolicy")]
    [JsonPropertyName("1043")]
    public _1043? _1043 { get; set; }       // "name": "IT policy - I have read and understood"

    [SqlName("EmploymentType")]
    [JsonPropertyName("1046")]
    public _1046? _1046 { get; set; }       // "name": "Employment type"

    [SqlName("OfficeAddress")]
    [JsonPropertyName("1068")]
    public _1068? _1068 { get; set; }       // "name": "Office - Address"

    [SqlName("CostCenter")]
    [JsonPropertyName("1080")]
    public _1080? _1080 { get; set; }       // "name": "Cost Center"

    [SqlName("DatumFörAvtalsändring")]
    [JsonPropertyName("1150")]              
    public _1150? _1150 { get; set; }       // "name": "Datum för avtalsändring"

    [SqlName("AnställningsdatumKoncern")]
    [JsonPropertyName("1166")]
    public _1166? _1166 { get; set; }       // "name": "Anställningsdatum koncern"

    [SqlName("AlternativtStartdatum")]
    [JsonPropertyName("1173")]
    public _1173? _1173 { get; set; }       // "name": "Alternativt startdatum"

    [SqlName("Verksamhetsområde")]
    [JsonPropertyName("1175")]
    public _1175? _1175 { get; set; }       // "name": "Verksamhetsområde"

    [SqlName("Organisationsnummer")]
    [JsonPropertyName("1189")]
    public _1189? _1189 { get; set; }       // "name": "Organisationsnummer"

    [SqlName("TelefonArbete")]
    [JsonPropertyName("1213")]
    public _1213? _1213 { get; set; }       // "name": "Telefon arbete"

    [SqlName("Chefsenhet")]
    [JsonPropertyName("1218")]
    public _1218? _1218 { get; set; }       // "name": "Chefsenhet"

    [SqlName("LösenordMicrosoft365")]
    [JsonPropertyName("1237")]
    public _1237? _1237 { get; set; }       // "name": "Lösenord Microsoft 365"

    [SqlName("ThorenID")]
    [JsonPropertyName("1251")]
    public _1251? _1251 { get; set; }       // "name": "ThorenID"

    [SqlName("AnvändarnamnSchoolsoft")]
    [JsonPropertyName("1321")]
    public _1321? _1321 { get; set; }       // "name": "Användarnamn Schoolsoft"

    [JsonPropertyName("1322")]
    public _1322? _1322 { get; set; }       // "name": "Lösenord Schoolsoft"

    [SqlName("AnvändarnamnGoogle")]
    [JsonPropertyName("1323")]
    public _1323? _1323 { get; set; }       // "name": "Användarnamn Google"

    [SqlName("LösenordGoogle")]
    [JsonPropertyName("1324")]
    public _1324? _1324 { get; set; }       // "name": "Lösenord Google"

}

public class Data
{
    public string? value { get; set; }
    public string? guid { get; set; }
    public string? uniqueImportId { get; set; }
    public string? alternativeExportValue { get; set; }
    public string? profileGUID { get; set; }
    public string? username { get; set; }
    public string? employeeId { get; set; }
    public string? profileId { get; set; }
}

public class _1321
{
    public string? name { get; set; }
    public string? lastModified { get; set; }
    public string? dataValidFrom { get; set; }
    public string? type { get; set; }
    public string? typeId { get; set; }
    public Data? data { get; set; }
    public string? visible { get; set; }
    public Visibility? visibility { get; set; }
}

public class _1322
{
    public string? name { get; set; }
    public string? lastModified { get; set; }
    public string? dataValidFrom { get; set; }
    public string? type { get; set; }
    public string? typeId { get; set; }
    public Data? data { get; set; }
    public string? visible { get; set; }
    public Visibility? visibility { get; set; }
}
public class _1323
{
    public string? name { get; set; }
    public string? lastModified { get; set; }
    public string? dataValidFrom { get; set; }
    public string? type { get; set; }
    public string? typeId { get; set; }
    public Data? data { get; set; }
    public string? visible { get; set; }
    public Visibility? visibility { get; set; }
}
public class _1324
{
    public string? name { get; set; }
    public string? lastModified { get; set; }
    public string? dataValidFrom { get; set; }
    public string? type { get; set; }
    public string? typeId { get; set; }
    public Data? data { get; set; }
    public string? visible { get; set; }
    public Visibility? visibility { get; set; }
}

public class _50
{
    public string? name { get; set; }
    public string? lastModified { get; set; }
    public string? dataValidFrom { get; set; }
    public string? type { get; set; }
    public string? typeId { get; set; }
    public Data? data { get; set; }
    public string? visible { get; set; }
    public Visibility? visibility { get; set; }
    public Plannedchange? plannedChange { get; set; }
}


public class _1189
{
    public string? name { get; set; }
    public string? lastModified { get; set; }
    public string? dataValidFrom { get; set; }
    public string? type { get; set; }
    public string? typeId { get; set; }
    public Data? data { get; set; }
    public string? visible { get; set; }
}


public class _1166
{
    public string? name { get; set; }
    public string? lastModified { get; set; }
    public string? dataValidFrom { get; set; }
    public string? type { get; set; }
    public string? typeId { get; set; }
    public Data? data { get; set; }
    public string? visible { get; set; }
}

public class _1251
{
    public string? name { get; set; }
    public string? lastModified { get; set; }
    public string? dataValidFrom { get; set; }
    public string? type { get; set; }
    public string? typeId { get; set; }
    public Data? data { get; set; }
    public string? visible { get; set; }
}


public class _1043
{
    public string? name { get; set; }
    public string? lastModified { get; set; }
    public string? dataValidFrom { get; set; }
    public string? type { get; set; }
    public string? typeId { get; set; }
    public Datum[]? data { get; set; }
    public string? visible { get; set; }
}


public class _45
{
    public string? name { get; set; }
    public string? lastModified { get; set; }
    public string? dataValidFrom { get; set; }
    public string? type { get; set; }
    public string? typeId { get; set; }
    public Data? data { get; set; }
    public string? visible { get; set; }
}

public class _32
{
    public string? name { get; set; }
    public string? lastModified { get; set; }
    public string? dataValidFrom { get; set; }
    public string? type { get; set; }
    public string? typeId { get; set; }
    public Data? data { get; set; }
    public string? visible { get; set; }
}

public class _1237
{
    public string? name { get; set; }
    public string? lastModified { get; set; }
    public string? dataValidFrom { get; set; }
    public string? type { get; set; }
    public string? typeId { get; set; }
    public Data? data { get; set; }
    public string? visible { get; set; }
}

public class _1028
{
    public string? name { get; set; }
    public string? lastModified { get; set; }
    public string? dataValidFrom { get; set; }
    public string? type { get; set; }
    public string? typeId { get; set; }
    public Data? data { get; set; }
    public string? visible { get; set; }
    public Plannedchangeaudit[]? plannedChangeAudit { get; set; }
    public Plannedchange? plannedChange { get; set; }
}
public class _1080
{
    public string? name { get; set; }
    public string? lastModified { get; set; }
    public string? dataValidFrom { get; set; }
    public string? type { get; set; }
    public string? typeId { get; set; }
    public Data? data { get; set; }
    public string? visible { get; set; }
}

public class _1005
{
    public string? name { get; set; }
    public string? lastModified { get; set; }
    public string? dataValidFrom { get; set; }
    public string? type { get; set; }
    public string? typeId { get; set; }
    public Data? data { get; set; }
    public string? visible { get; set; }
}

public class _15
{
    public string? name { get; set; }
    public string? lastModified { get; set; }
    public string? dataValidFrom { get; set; }
    public string? type { get; set; }
    public string? typeId { get; set; }
    public Data? data { get; set; }
    public string? visible { get; set; }
    public Plannedchange? plannedChange { get; set; }
    public Plannedchangeaudit[]? plannedChangeAudit { get; set; }
}

public class Plannedchangeaudit
{
    public string? lastModified { get; set; }
    public string? dataValidFrom { get; set; }
    public string? dataValidTo { get; set; }
    public string? status { get; set; }
    public Data? data { get; set; }
}

public class _1150
{
    public string? name { get; set; }
    public string? lastModified { get; set; }
    public string? dataValidFrom { get; set; }
    public string? type { get; set; }
    public string? typeId { get; set; }
    public Data? data { get; set; }
    public string? visible { get; set; }
}

public class _1068
{
    public string? name { get; set; }
    public string? lastModified { get; set; }
    public string? type { get; set; }
    public string? typeId { get; set; }
    public Data? data { get; set; }
    public string? visible { get; set; }
}

public class _1002
{
    public string? name { get; set; }
    public string? lastModified { get; set; }
    public string? dataValidFrom { get; set; }
    public string? type { get; set; }
    public string? typeId { get; set; }
    public Data? data { get; set; }
    public string? visible { get; set; }
}

public class _1033
{
    public string? name { get; set; }
    public string? lastModified { get; set; }
    public string? dataValidFrom { get; set; }
    public string? type { get; set; }
    public string? typeId { get; set; }
    public Datum[]? data { get; set; }
    public string? visible { get; set; }
}

public class Datum
{
    public string? guid { get; set; }
    public string? value { get; set; }
    public string? alternativeExportValue { get; set; }
}

public class _1173
{
    public string? name { get; set; }
    public string? lastModified { get; set; }
    public string? dataValidFrom { get; set; }
    public string? type { get; set; }
    public string? typeId { get; set; }
    public Data? data { get; set; }
    public string? visible { get; set; }
}

public class _1003
{
    public string? name { get; set; }
    public string? lastModified { get; set; }
    public string? dataValidFrom { get; set; }
    public string? type { get; set; }
    public string? typeId { get; set; }
    public Data? data { get; set; }
    public string? visible { get; set; }
}

public class _1175
{
    public string? name { get; set; }
    public string? lastModified { get; set; }
    public string? dataValidFrom { get; set; }
    public string? type { get; set; }
    public string? typeId { get; set; }
    public Data? data { get; set; }
    public string? visible { get; set; }
}

public class _1213
{
    public string? name { get; set; }
    public string? lastModified { get; set; }
    public string? dataValidFrom { get; set; }
    public string? type { get; set; }
    public string? typeId { get; set; }
    public Data? data { get; set; }
    public string? visible { get; set; }
}

public class _1218
{
    public string? name { get; set; }
    public string? lastModified { get; set; }
    public string? dataValidFrom { get; set; }
    public string? type { get; set; }
    public string? typeId { get; set; }
    public Data? data { get; set; }
    public string? visible { get; set; }
}
public class _101
{
    public string? name { get; set; }
    public string? lastModified { get; set; }
    public string? dataValidFrom { get; set; }
    public string? type { get; set; }
    public string? typeId { get; set; }
    public Data? data { get; set; }
    public string? visible { get; set; }
}

public class _8
{
    public string? name { get; set; }
    public string? lastModified { get; set; }
    public string? dataValidFrom { get; set; }
    public string? type { get; set; }
    public string? typeId { get; set; }
    public Data? data { get; set; }
    public string? visible { get; set; }
}

public class _37
{
    public string? name { get; set; }
    public string? lastModified { get; set; }
    public string? dataValidFrom { get; set; }
    public string? type { get; set; }
    public string? typeId { get; set; }
    public Data? data { get; set; }
    public string? visible { get; set; }
}

public class _14
{
    public string? name { get; set; }
    public string? lastModified { get; set; }
    public string? dataValidFrom { get; set; }
    public string? type { get; set; }
    public string? typeId { get; set; }
    public Data? data { get; set; }
    public string? visible { get; set; }
    public Plannedchange? plannedChange { get; set; }
}


public class Plannedchange
{
    public string? lastModified { get; set; }
    public string? dataValidFrom { get; set; }
    public string? dataValidTo { get; set; }
    public string? status { get; set; }
    public Data? data { get; set; }

}

public class _0
{
    public string? name { get; set; }
    public string? lastModified { get; set; }
    public string? dataValidFrom { get; set; }
    public string? type { get; set; }
    public string? typeId { get; set; }
    public Data? data { get; set; }
    public string? visible { get; set; }
}

public class _103
{
    public string? name { get; set; }
    public string? lastModified { get; set; }
    public string? dataValidFrom { get; set; }
    public string? type { get; set; }
    public string? typeId { get; set; }
    public Data? data { get; set; }
    public string? visible { get; set; }
}

    public class _1
{
    public string? name { get; set; }
    public string? lastModified { get; set; }
    public string? dataValidFrom { get; set; }
    public string? type { get; set; }
    public string? typeId { get; set; }
    public Data? data { get; set; }
    public string? visible { get; set; }
}


public class _2
{
    public string? name { get; set; }
    public string? lastModified { get; set; }
    public string? dataValidFrom { get; set; }
    public string? type { get; set; }
    public string? typeId { get; set; }
    public Data? data { get; set; }
    public string? visible { get; set; }
}


public class _7
{
    public string? name { get; set; }
    public string? lastModified { get; set; }
    public string? dataValidFrom { get; set; }
    public string? type { get; set; }
    public string? typeId { get; set; }
    public Data? data { get; set; }
    public string? visible { get; set; }
}

public class _3
{
    public string? name { get; set; }
    public string? lastModified { get; set; }
    public string? dataValidFrom { get; set; }
    public string? type { get; set; }
    public string? typeId { get; set; }
    public Data? data { get; set; }
    public string? visible { get; set; }
}

public class _19
{
    public string? name { get; set; }
    public string? lastModified { get; set; }
    public string? dataValidFrom { get; set; }
    public string? type { get; set; }
    public string? typeId { get; set; }
    public Data? data { get; set; }
    public string? visible { get; set; }
}

public class _40
{
    public string? name { get; set; }
    public string? lastModified { get; set; }
    public string? dataValidFrom { get; set; }
    public string? type { get; set; }
    public string? typeId { get; set; }
    public Data? data { get; set; }
    public string? visible { get; set; }
    public Visibility? visibility { get; set; }
    public Plannedchange? plannedChange { get; set; }
}

public class Visibility
{
    public string? lastModified { get; set; }
    public List<Planned>? planned { get; set; }
}

public class Planned
{
    public string? visible { get; set; }
    public string? lastModified { get; set; }
    public string? validFrom { get; set; }


}
public class _9
{
    public string? name { get; set; }
    public string? lastModified { get; set; }
    public string? dataValidFrom { get; set; }
    public string? type { get; set; }
    public string? typeId { get; set; }
    public Data? data { get; set; }
    public string? visible { get; set; }
}

public class _21
{
    public string? name { get; set; }
    public string? lastModified { get; set; }
    public string? dataValidFrom { get; set; }
    public string? type { get; set; }
    public string? typeId { get; set; }
    public Data? data { get; set; }
    public string? visible { get; set; }
}

public class _22
{
    public string? name { get; set; }
    public string? lastModified { get; set; }
    public string? dataValidFrom { get; set; }
    public string? type { get; set; }
    public string? typeId { get; set; }
    public Data? data { get; set; }
    public string? visible { get; set; }
}

public class _10
{
    public string? name { get; set; }
    public string? lastModified { get; set; }
    public string? dataValidFrom { get; set; }
    public string? type { get; set; }
    public string? typeId { get; set; }
    public List<Data?>? data { get; set; }
    public string? visible { get; set; }
}

public class _49
{
    public string? name { get; set; }
    public string? lastModified { get; set; }
    public string? dataValidFrom { get; set; }
    public string? type { get; set; }
    public string? typeId { get; set; }
    public Data? data { get; set; }
    public string? visible { get; set; }
}

public class _1046
{
    public string? name { get; set; }
    public string? lastModified { get; set; }
    public string? dataValidFrom { get; set; }
    public string? type { get; set; }
    public string? typeId { get; set; }
    public Data? data { get; set; }
    public string? visible { get; set; }
    public Plannedchange? plannedChange { get; set; }
}

public class _38
{
    public string? name { get; set; }
    public string? lastModified { get; set; }
    public string? dataValidFrom { get; set; }
    public string? type { get; set; }
    public string? typeId { get; set; }
    public Data? data { get; set; }
    public string? visible { get; set; }
}

public class _1029
{
    public string? name { get; set; }
    public string? lastModified { get; set; }
    public string? dataValidFrom { get; set; }
    public string? type { get; set; }
    public string? typeId { get; set; }
    public Data? data { get; set; }
    public string? visible { get; set; }
    public Visibility? visibility { get; set; }
}


public class _47
{
    public string? name { get; set; }
    public string? lastModified { get; set; }
    public string? dataValidFrom { get; set; }
    public string? type { get; set; }
    public string? typeId { get; set; }
    public Data? data { get; set; }
    public string? visible { get; set; }
}