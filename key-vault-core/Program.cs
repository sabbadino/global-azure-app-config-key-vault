using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure.Extensions.AspNetCore.Configuration.Secrets;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;

namespace key_vault_core
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

      

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                    webBuilder.ConfigureAppConfiguration((hostingContext, config) =>
                        {
                            var settings = config.Build();

                            var cnstring = settings["appConfigurationConnectionString"];

                            config.AddAzureAppConfiguration(options =>
                            {

                                options.Connect(cnstring)
                                // ignore this for the moment
                                    .ConfigureKeyVault(kv =>
                                    {
                                        kv.SetCredential(new DefaultAzureCredential());
                                    })
                                    // ignore this for the moment
                                    .ConfigureRefresh(refresh =>
                                    {
                                        refresh.Register("SettingsGroup:Sentinel", refreshAll: true)
                                               .SetCacheExpiration(TimeSpan.FromSeconds(10));
                                    })
                                   
                                ;

                            },optional:false);
                        })
                        .UseStartup<Startup>());
    }
 
}
