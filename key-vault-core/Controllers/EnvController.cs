using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.FeatureManagement;
using Microsoft.FeatureManagement.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace key_vault_core.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EnvController : ControllerBase
    {
        private readonly SettingsGroup _appSettings;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly IConfiguration _configuration;

        public EnvController(IOptionsSnapshot<SettingsGroup> appSettings, IWebHostEnvironment hostingEnvironment, IConfiguration configuration)
        {
            _appSettings = appSettings.Value;
            _hostingEnvironment = hostingEnvironment;
            _configuration = configuration;
        }


        [HttpGet("envs")]
        public GetEnvsResponse GetEnvs()
        {
            int appConfigRefreshInSeconds;
            if (!int.TryParse(_configuration["app-config-refresh-in-seconds"], out appConfigRefreshInSeconds))
            {
                appConfigRefreshInSeconds = 15;
            }
            return new GetEnvsResponse
            {
                RefreshInSeconds= appConfigRefreshInSeconds, Sentinel = _configuration["Sentinel"] , Environment = _hostingEnvironment.EnvironmentName, Settings = _appSettings ,
                AppConfigUrl = _configuration["appConfigurationEndpoint"]

            };
        }


    }

    public class GetEnvsResponse
    {
        public string AppConfigUrl { get; set; }
        public int RefreshInSeconds { get; set; }
        public string Sentinel { get; set; }
        public string Environment { get; set; }
        public SettingsGroup Settings { get; set; }
    }
}
