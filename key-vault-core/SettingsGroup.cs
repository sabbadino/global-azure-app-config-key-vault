using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace key_vault_core
{

   


public class SettingsGroup
    {
        public string Sentinel { get; set; }
        
        public string Key1 { get; set; }
        public SomeJSon Key2 { get; set; }

      
    }

    public class SomeJSon
    {
        public int FontSize { get; set; }
        public bool UseDefaultRouting { get; set; }
    }
}
