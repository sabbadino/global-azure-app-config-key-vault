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
        private readonly IFeatureManager _featureManager;

        // do not use IOptions<SettingsGroup> : it  does not get thre changed values 
        public EnvController(IOptionsSnapshot<SettingsGroup> appSettings, IFeatureManager featureManager)
        {
            _appSettings = appSettings.Value;
            _featureManager = featureManager;
        }

     
        [HttpGet("envs")]
        public SettingsGroup GetEnvs()
        {
            return _appSettings;
        }

        [HttpGet("features")]
        public bool Features()
        {
            return _featureManager.IsEnabledAsync("feature1").Result;
        }

        [FeatureGate("feature1")]
        [HttpGet("feature1")]
        public bool feature1()
        {
            return _featureManager.IsEnabledAsync("feature1").Result;
        }
    }
}
