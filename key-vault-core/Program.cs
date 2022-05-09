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
                            // how to plug AKV as config provider directly 
                            //var settings = config.Build();
                            //var keyVaultUrl = settings.GetValue<string>("keyVaultUrl");
                            //var keyVaultEndpoint = new Uri(keyVaultUrl);

                            //config.AddAzureKeyVault(
                            //    keyVaultEndpoint,
                            //    new DefaultAzureCredential(), new AzureKeyVaultConfigurationOptions { ReloadInterval = new TimeSpan(1, 0, 0) });

                            var settings = config.Build();
                            var connectionString = settings["appConfigurationEndpoint"];
                            if(!int.TryParse(settings["app-config-refresh-in-seconds"], out var appConfigRefreshInSeconds)) {
                                appConfigRefreshInSeconds = 15;
                            }




                            config.AddAzureAppConfiguration(options =>
                            {
                                options.Connect(new Uri(connectionString), new DefaultAzureCredential())
                                     .ConfigureKeyVault(kv =>
                                     {
                                         kv.SetCredential(new DefaultAzureCredential());

                                         // this is supopsed to  work in all scenarios 
                                         //https://docs.microsoft.com/en-us/dotnet/api/azure.identity.defaultazurecredential?view=azure-dotnet
                                         //                                The following credential types if enabled will be tried, in order:
                                         //                                  EnvironmentCredential
                                         //                                  ManagedIdentityCredential
                                         //                                  SharedTokenCacheCredential
                                         //                                  VisualStudioCredential
                                         //                                  VisualStudioCodeCredential
                                         //                                  AzureCliCredential
                                         //                                  AzurePowerShellCredential
                                         //                                  InteractiveBrowserCredential
                                         //options.Connect(new Uri(cnstring), new DefaultAzureCredential())


                                     })
                                    .ConfigureRefresh(refresh =>
                                    {
                                        refresh.Register("Sentinel", refreshAll: true)
                                               .SetCacheExpiration(TimeSpan.FromSeconds(appConfigRefreshInSeconds));
                                    })

                                ;

                            }, optional: false);
                        })
                        .UseStartup<Startup>());
    }
 
}
