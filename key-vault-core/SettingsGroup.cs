using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace key_vault_core
{




    public class SettingsGroup
    {

        public string FromAppLocalSecret { get; set; }
        public string FromAkv { get; set; }

        public string FromAkvEncryptedInGit { get; set; }

        public string FromAppSettingsFile { get; set; }
        public string FromAppConfigurationSimple { get; set; }
        public ComplexData FromAppConfigurationComplex { get; set; }


    }

    public class ComplexData
    {
        public int FontSize { get; set; }
        public bool UseDefaultRouting { get; set; }
    }
}
