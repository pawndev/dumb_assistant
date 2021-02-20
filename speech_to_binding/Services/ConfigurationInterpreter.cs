using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nett;
using speech_to_binding.Models;

namespace speech_to_binding.Services
{
    class ConfigurationInterpreter
    {
        public string configurationPath;
        private Configuration _configuration;

        public ConfigurationInterpreter(string configStr)
        {
            this.configurationPath = configStr;
            this._configuration = Toml.ReadString<Configuration>(configStr);
        }

        public Configuration GetConfiguration()
        {
            return this._configuration;
        }
    }
}
