using System.Text.Json;
using System.Text;
using System.Xml.Serialization;
using System.Net;
using System.Text.RegularExpressions;
using System.Text.Encodings.Web;
using System.Reflection.PortableExecutable;
using System.Net.Http.Headers;
using System.Text.Json.Serialization;
using System.Reflection.Metadata;

namespace APIs;

public class CatalystAPI
{
    private string ClientId { get; } = string.Empty;
    private string ClientSecret { get; } = string.Empty;
    private CatalystAccess? Access = null;
    public event Action<string>? OnProgress;
    private readonly HttpClient Client = new();
    private readonly HttpClient ClientAccess = new();
    public string CacheDir { get; }
    public bool UseCache { get; set; } = true;
    public readonly string AccessTokenUrl = "https://thorengruppen.catalystone.com/mono/api/accesstoken";
    //public readonly string EmployeesUrl =   "https://thorengruppen.catalystone.com/mono/api/employees";
    //public readonly string EmployeesUrl = "https://thorengruppen.catalystone.com/mono/api/employees?fullObject=true&includeInactive=true";
    public readonly string EmployeesUrl = "https://thorengruppen.catalystone.com/mono/api/employees?fullObject=true";
    public readonly string ModifiedSinceUrl = "https://thorengruppen.catalystone.com/mono/api/employees?fullObject=true?modifiedSince=";
    public readonly string OrgsUrl =        "https://thorengruppen.catalystone.com/mono/api/organizations";

    private CatalystAPI(string CredsDir, Action<string>? OnProgressListener = null, bool UseCache=true)
    {
        OnProgress = OnProgressListener;
        CacheDir = Path.Combine(Directory.GetCurrentDirectory(), "cache");
        Directory.CreateDirectory(CacheDir);
        ClientId = File.ReadAllText(Path.Combine(CredsDir, "Catalyst_ClientId.txt"));
        ClientSecret = File.ReadAllText(Path.Combine(CredsDir, "Catalyst_ClientSecret.txt"));

        ClientAccess.DefaultRequestHeaders.Add("accept", "application/json");
        ClientAccess.DefaultRequestHeaders.Add("Api-Version", "v3");
        ClientAccess.DefaultRequestHeaders.Add("Grant-Type", "client_credentials");
        ClientAccess.DefaultRequestHeaders.Add("Client-Id", ClientId);
        ClientAccess.DefaultRequestHeaders.Add("Client-Secret", ClientSecret);
        ClientAccess.DefaultRequestHeaders.Add("formatResponse", "true");
    }

    public static Task<CatalystAPI> CreateAsync(string CredsDir, Action<string>? OnProgressListener = null, bool UseCache = true)
    {
        var ret = new CatalystAPI(CredsDir, OnProgressListener, UseCache);
        return ret.InitializeAsync();
    }

    public async Task<CatalystAPI> InitializeAsync()
    {
        try
        {
            string responseBody = await ClientAccess.GetStringAsync(AccessTokenUrl);
            //OnProgress?.Invoke($"{responseBody}");
            Access = JsonSerializer.Deserialize<CatalystAccess>(responseBody);
        }
        catch (HttpRequestException e)
        {
            OnProgress?.Invoke("\nException Caught!");
            OnProgress?.Invoke($"Message :{e.Message} ");
        }
        return this;
    }

    public async Task<bool> SetEmployeeSG(string guid, string profileGuid, string schoolsoftUsername, string schoolsoftPassword,
                                            string googleUsername, string googlePassword)
    {
        if (Access is null)
        {
            OnProgress?.Invoke("Calling GetOrgAsync() with no accesstoken");
            return false;
        }
        try
        {
            OnProgress?.Invoke($"SetEmployeeSG()");
            Client.DefaultRequestHeaders.Clear();
            Client.DefaultRequestHeaders.Add("Access-Token", Access.access_token);
            Client.DefaultRequestHeaders.Add("Api-Version", "v3");

            var employees = new CatalystEmployees();
            employees.employees = new List<Employee?>();
            var employee = new Employee();
            employee.guid = guid;
            employee.profileGUID = profileGuid;
            employee.field = new Field();
            employee.field._1321 = new _1321();
            employee.field._1321.data = new Data();
            employee.field._1321.data.value = schoolsoftUsername;
            employee.field._1322 = new _1322();
            employee.field._1322.data = new Data();
            employee.field._1322.data.value = schoolsoftPassword;
            employee.field._1323 = new _1323();
            employee.field._1323.data = new Data();
            employee.field._1323.data.value = googleUsername;
            employee.field._1324 = new _1324();
            employee.field._1324.data = new Data();
            employee.field._1324.data.value = googlePassword;
            employees.employees.Add(employee);
            var options = new JsonSerializerOptions
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };
            // "https://thorengruppen.catalystone.com/mono/api/employees"
            using StringContent jsonContent = new(
                JsonSerializer.Serialize(employees, options),
                Encoding.UTF8,
                new MediaTypeHeaderValue("application/json"));
            using HttpResponseMessage response = await Client.PatchAsync("https://thorengruppen.catalystone.com/mono/api/employees", jsonContent);
            var jsonResponse = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"{jsonResponse}\n");
            //[15:54:41 INF] SetEmployeeSG()
            //{ "message":"Invalid access token"}
        }
        catch (HttpRequestException e)
        {
            OnProgress?.Invoke("\nException Caught!");
            OnProgress?.Invoke($"Message :{e.Message} ");
            return false;
        }
        return true;
    }

    public async Task<CatalystEmployees?> GetModifiedAsync(DateTime fromDate)
    {
        if (Access is null)
        {
            OnProgress?.Invoke("Calling GetChangesAsync() with no accesstoken");
            return null;
        }
        try
        {
            Client.DefaultRequestHeaders.Add("Access-Token", Access.access_token);
            Client.DefaultRequestHeaders.Add("Api-Version", "v3");
            string filename = Path.Combine(CacheDir, $"Changes.json");

            var actualUrl = ModifiedSinceUrl + fromDate.ToString("s") + "+01:00";
            //string sFieldFilter = "&fieldId=0&fieldId=1&fieldId=2&fieldId=3&fieldId=7&fieldId=9&fieldId=14&fieldId=15&fieldId=22&fieldId=32&fieldId=37&fieldId=38&fieldId=45&fieldId=49&fieldId=101&fieldId=103&fieldId=1003&fieldId=1028&fieldId=1029&fieldId=1033&fieldId=1043&fieldId=1080&fieldId=1173&fieldId=1175&fieldId=1213&fieldId=1251";
            //string sFieldFilter = "&fieldId=2&fieldId=3&fieldId=7&fieldId=14";
            string reqUrl = $"https://thorengruppen.catalystone.com/mono/api/employees?fullObject=true&includeInactive=true&modifiedSince={fromDate.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'")}";
            OnProgress?.Invoke(reqUrl);
            string responseBody = await Client.GetStringAsync(reqUrl);

            //File.WriteAllText(filename, responseBody);                
            //OnProgress?.Invoke($"{responseBody}");

            using JsonDocument document = JsonDocument.Parse(responseBody);
            var options = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                WriteIndented = true
            };
            var formattedJson = JsonSerializer.Serialize(document, options);
            File.WriteAllText(filename, formattedJson);
            //WriteFieldName(document, Path.Combine(CacheDir, $"FieldName.txt"))
            var employees = JsonSerializer.Deserialize<CatalystEmployees>(responseBody);
            return employees;
        }
        catch (HttpRequestException e)
        {
            OnProgress?.Invoke("\nException Caught!");
            OnProgress?.Invoke($"Message :{e.Message} ");
            return null;
        }
    }

    public void WriteFieldName(JsonDocument document, string filename)
    {
        var employees = document.RootElement.GetProperty("employees");
    }

    public async Task<CatalystEmployees?> GetEmployeesAsync()
    {
        if(Access is null)
        {
            OnProgress?.Invoke("Calling GetEmployees() with no accesstoken");
            return null;
        }
        try
        {
            Client.DefaultRequestHeaders.Add("Access-Token", Access.access_token);
            Client.DefaultRequestHeaders.Add("Api-Version", "v3");
            string filename = Path.Combine(CacheDir, "Employees.json");
            string responseBody = await Client.GetStringAsync(EmployeesUrl);
            //File.WriteAllText(filename, responseBody);                
            //OnProgress?.Invoke($"{responseBody}");

            using JsonDocument document = JsonDocument.Parse(responseBody);
            var options = new JsonSerializerOptions {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                WriteIndented = true
            };
            var formattedJson = JsonSerializer.Serialize(document, options);
            File.WriteAllText(filename, formattedJson);
            var employees = JsonSerializer.Deserialize<CatalystEmployees>(responseBody);
            return employees;
        }
        catch (HttpRequestException e)
        {
            OnProgress?.Invoke("\nException Caught!");
            OnProgress?.Invoke($"Message :{e.Message} ");
            return null;
        }
    }
    public async Task<CatalystOrgs?> GetOrgAsync()
    {
        if (Access is null)
        {
            OnProgress?.Invoke("Calling GetOrgAsync() with no accesstoken");
            return null;
        }
        try
        {
            OnProgress?.Invoke($"GetOrgAsync()");
            Client.DefaultRequestHeaders.Clear();
            Client.DefaultRequestHeaders.Add("Access-Token", Access.access_token);
            Client.DefaultRequestHeaders.Add("Api-Version", "v3");
            Client.DefaultRequestHeaders.Add("includeSubOrg", "true");

            string filename = Path.Combine(CacheDir, "Orgs.json");
            string responseBody = await Client.GetStringAsync(OrgsUrl);
            File.WriteAllText("temp.txt", responseBody);                
            //OnProgress?.Invoke($"{responseBody}");

            using JsonDocument document = JsonDocument.Parse(responseBody);
            var options = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                WriteIndented = true
            };
            var formattedJson = JsonSerializer.Serialize(document, options);
            File.WriteAllText(filename, formattedJson);
            var orgs = JsonSerializer.Deserialize<CatalystOrgs>(responseBody);
            return orgs;
        }
        catch (HttpRequestException e)
        {
            OnProgress?.Invoke("\nException Caught!");
            OnProgress?.Invoke($"Message :{e.Message} ");
            return null;
        }
    }

}