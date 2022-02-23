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
        private readonly AppSettings _appSettings;
        public EnvController(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
        }

     
        [HttpGet("envs")]
        public AppSettings GetEnvs()
        {
            return _appSettings;
        }
    }
}
