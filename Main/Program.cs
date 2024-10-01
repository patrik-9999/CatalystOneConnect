using Serilog;
using Microsoft.Extensions.Configuration;
using Serilog.Core;
using System.Security.Cryptography;
using System.Linq;

using APIs;
using System.Xml.XPath;
using System;
using static System.Net.Mime.MediaTypeNames;
using System.Xml.Serialization;
using System.Linq.Expressions;


namespace Main;

public class Program
{
    static async Task Main(string[] args)
    {
        IConfigurationRoot config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();

        Settings settings = config.GetRequiredSection("Settings").Get<Settings>() ?? throw new Exception("settings is null");
        int daysBeforeStartCreateAccount = settings.DaysBeforeStartCreateAccount;

        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(config)
            .CreateLogger();

        Log.Information("Hämtar konton från CatalystOne...");
        var catalyst = await CatalystAPI.CreateAsync(
            CredsDir: settings.CredentialsDir,
            OnProgressListener: delegate (String message)
            {
                Log.Information(message);
            },
            UseCache: true);

        var res = await catalyst.GetModifiedAsync(DateTime.Now.AddDays(-daysBeforeStartCreateAccount));
        if (res is null || res.employees is null)
        {
            Log.Error("Tomt svar från catalyst.GetEmployeesAsync()");
            return;
        }
        Log.Information("Konton från CatalystOne: {count}", res.employees.Count);
        foreach (var employee in res.employees.Take(10))
        {
            Log.Information("Konto: {name} {email}", employee?.name, employee?.field?._7?.data?.value);
        }
    }
}


