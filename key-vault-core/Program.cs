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

namespace key_vault_core
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        //public static IHostBuilder CreateHostBuilder(string[] args) =>
        //    Host.CreateDefaultBuilder(args)
        //        .ConfigureAppConfiguration((context, config) =>
        //        {
        //            var versionPrefix = context.HostingEnvironment.EnvironmentName;
        //            var settings = config.Build();
        //            var keyVaultUrl = settings.GetValue<string>("keyVaultUrl");
        //            var keyVaultEndpoint = new Uri(keyVaultUrl);

        //            config.AddAzureKeyVault(
        //                keyVaultEndpoint,
        //                // new DefaultAzureCredential(),   new AzureKeyVaultConfigurationOptions  { ReloadInterval =new TimeSpan(1,0,0)});
        //                new DefaultAzureCredential(), new PrefixKeyVaultSecretManager(versionPrefix));
        //        })
        //        .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
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
                                    //.ConfigureKeyVault(kv =>
                                    //{
                                    //    kv.SetCredential(new DefaultAzureCredential());
                                    //})
                                    ;
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
