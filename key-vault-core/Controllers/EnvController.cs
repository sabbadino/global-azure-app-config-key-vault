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

namespace key_vault_core.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EnvController : ControllerBase
    {
        private readonly SettingsGroup _appSettings;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public EnvController(IOptions<SettingsGroup> appSettings, IWebHostEnvironment hostingEnvironment)
        {
            _appSettings = appSettings.Value;
            _hostingEnvironment = hostingEnvironment;
        }


        [HttpGet("envs")]
        public GetEnvsResponse GetEnvs()
        {
            return new GetEnvsResponse { Environment = _hostingEnvironment.EnvironmentName, Settings = _appSettings };
        }


    }

    public class GetEnvsResponse
    {
        public string Environment { get; set; }
        public SettingsGroup Settings { get; set; }
    }
}
