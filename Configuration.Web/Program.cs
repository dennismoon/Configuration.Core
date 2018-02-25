using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Configuration.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((context, config) =>
                {
                    var env = context.HostingEnvironment;

                    config.SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                        .AddEnvironmentVariables();

                    if (env.IsDevelopment())
                    {
                        // For more details on using the user secret store see http://go.microsoft.com/fwlink/?LinkID=532709
                        config.AddUserSecrets<Startup>();
                    }

                    var builtConfig = config.Build();

                    var vault = builtConfig["AzureKeyVault:Vault"];
                    var appClientId = builtConfig["AzureKeyVault:ClientId"];
                    var appClientSecret = builtConfig["AzureKeyVault:ClientSecret"];

                    config.AddAzureKeyVault($"https://{vault}.vault.azure.net/", appClientId, appClientSecret);
                })
                .UseApplicationInsights()
                .UseStartup<Startup>()
                .Build();
    }
}
