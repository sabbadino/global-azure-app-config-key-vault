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
                            var oriSettings = new List<IConfigurationSource>(config.Sources);
                            config.Sources.Clear();
                            var cnstring = settings["appConfigurationEndpoint"];
                            int appConfigRefreshInSeconds;
                            if (!int.TryParse(settings["app-config-refresh-in-seconds"], out appConfigRefreshInSeconds))
                            {
                                appConfigRefreshInSeconds = 15;
                            }
                            config.AddAzureAppConfiguration(options =>
                            {
#if DEBUG
                                options.Connect(new Uri(cnstring), new InteractiveBrowserCredential())
#else
                                options.Connect(new Uri(cnstring), new ManagedIdentityCredential())
#endif
                                     .ConfigureKeyVault(kv =>
                                     {
#if DEBUG
                                         kv.SetCredential(new InteractiveBrowserCredential());
#else
                                         kv.SetCredential(new ManagedIdentityCredential());
#endif

                                         // this is supopsed to  work in all scenarios if you are logged in the same tenant where app configuration / akv is 
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

                            oriSettings.ForEach(s=>
                            {
                                config.Add(s);
                            });
                        })
                        .UseStartup<Startup>());
    }
    public class PrefixKeyVaultSecretManager : KeyVaultSecretManager
    {
        private readonly string _prefix;

        public PrefixKeyVaultSecretManager(string prefix)
        {
            _prefix = $"{prefix}-";
        }

        public override bool Load(SecretProperties secret)
        {
            return secret.Name.StartsWith(_prefix);
        }

        public override string GetKey(KeyVaultSecret secret)
        {
            return secret.Name
                .Substring(_prefix.Length)
                .Replace("--", ConfigurationPath.KeyDelimiter);
        }
    }
}
