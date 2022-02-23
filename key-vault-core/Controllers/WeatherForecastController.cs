using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace key_vault_core.Controllers
{
   [ApiController]
    [Route("[controller]")]
    public class EnvController : ControllerBase
    {
        private readonly SettingsGroup _appSettings;
        public EnvController(IOptions<SettingsGroup> appSettings)
        {
            _appSettings = appSettings.Value;
        }

     
        [HttpGet("envs")]
        public SettingsGroup GetEnvs()
        {
            return _appSettings;
        }
    }
}
