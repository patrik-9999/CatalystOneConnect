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
using Serilog;

namespace APIs;

public class CatalystAPI
{
    public event Action<string>? OnProgress;
    public string CacheDir { get; }
    public bool UseCache { get; set; }
    public readonly string AccessTokenUrl;
    public readonly string EmployeesUrl;
    public readonly string OrgsUrl;
    private string WebApp { get; }
    private string ClientId { get; }
    private string ClientSecret { get; }
    private CatalystAccess? Access = null;
    private readonly HttpClient Client = new();
    private readonly HttpClient ClientAccess = new();
    static readonly string AccessTokenUrlTemplate = "https://{0}.catalystone.com/mono/api/accesstoken";
    static readonly string EmployeesUrlTemplate = "https://{0}.catalystone.com/mono/api/employees?fullObject=true";
    static readonly string OrgsUrlTemplate = "https://{0}.catalystone.com/mono/api/organizations";
    static readonly string ModifiedSinceUrlTemplate = "https://{0}.catalystone.com/mono/api/employees?fullObject=true&includeInactive=true&modifiedSince={1}";

    private CatalystAPI(string credsDir, Action<string>? OnProgressListener = null, bool useCache=true)
    {
        OnProgress = OnProgressListener;
        CacheDir = Path.Combine(Directory.GetCurrentDirectory(), "cache");
        Directory.CreateDirectory(CacheDir);
        ClientId = File.ReadAllText(Path.Combine(credsDir, "Catalyst_ClientId.txt"));
        ClientSecret = File.ReadAllText(Path.Combine(credsDir, "Catalyst_ClientSecret.txt"));
        WebApp = File.ReadAllText(Path.Combine(credsDir, "Catalyst_WebApp.txt"));
        UseCache = useCache;
        AccessTokenUrl = string.Format(AccessTokenUrlTemplate, WebApp);
        EmployeesUrl = string.Format(EmployeesUrlTemplate, WebApp);
        //ModifiedSinceUrl = string.Format(ModifiedSinceUrlTemplate, WebApp);
        OrgsUrl = string.Format(OrgsUrlTemplate, WebApp);

        ClientAccess.DefaultRequestHeaders.Add("accept", "application/json");
        ClientAccess.DefaultRequestHeaders.Add("Api-Version", "v3");
        ClientAccess.DefaultRequestHeaders.Add("Grant-Type", "client_credentials");
        ClientAccess.DefaultRequestHeaders.Add("Client-Id", ClientId);
        ClientAccess.DefaultRequestHeaders.Add("Client-Secret", ClientSecret);
        ClientAccess.DefaultRequestHeaders.Add("formatResponse", "true");
    }

    public static Task<CatalystAPI> CreateAsync(string credsDir, Action<string>? OnProgressListener = null, bool useCache = true)
    {
        var ret = new CatalystAPI(credsDir, OnProgressListener, useCache);
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
            OnProgress?.Invoke($"Message :{e.Message} ");
        }
        return this;
    }


    // Updates CatalystOne with the given values for the employee with the given guid and profileGuid.
    public async Task<bool> SetEmployeeSG(string guid, string profileGuid, string schoolsoftUsername, string schoolsoftPassword,
                                          string googleUsername, string googlePassword)
    {
        if (Access is null)
        {
            OnProgress?.Invoke("Calling SetEmployeeSG(() with no accesstoken");
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
            string modifiedSinceUrl = string.Format(ModifiedSinceUrlTemplate, WebApp, fromDate.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"));
            OnProgress?.Invoke(modifiedSinceUrl);
            string responseBody = await Client.GetStringAsync(modifiedSinceUrl);

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

    public async Task<CatalystEmployees?> GetEmployeesAsync()
    {
        string cacheFile = Path.Combine(CacheDir, "Employees.json");
        if (UseCache && File.Exists(cacheFile))
        {
            if (JsonSerializer.Deserialize<CatalystEmployees>(File.ReadAllText(cacheFile)) is CatalystEmployees employees)
            {
                OnProgress?.Invoke("Reading from cache");
                return employees;
            }
        }
        if (Access is null)
        {
            OnProgress?.Invoke("Calling GetEmployeesAsync() with no accesstoken");
            return null;
        }
        try
        {
            Client.DefaultRequestHeaders.Add("Access-Token", Access.access_token);
            Client.DefaultRequestHeaders.Add("Api-Version", "v3");
            
            string responseBody = await Client.GetStringAsync(EmployeesUrl);
            //File.WriteAllText(filename, responseBody);                
            //OnProgress?.Invoke($"{responseBody}");

            using JsonDocument document = JsonDocument.Parse(responseBody);
            var options = new JsonSerializerOptions {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                WriteIndented = true
            };
            var formattedJson = JsonSerializer.Serialize(document, options);
            File.WriteAllText(cacheFile, formattedJson);
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