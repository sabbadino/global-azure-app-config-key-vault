using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.FeatureManagement;
using Microsoft.FeatureManagement.Mvc;

namespace key_vault_core.Controllers
{
   [ApiController]
    [Route("[controller]")]
    public class EnvController : ControllerBase
    {
        private readonly SettingsGroup _appSettings;

        // IMPORTANT do not use IOptions<SettingsGroup> : it  does not get the changed values 
        public EnvController(IOptionsSnapshot<SettingsGroup> appSettings)
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
