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

        public ConfigurationInterpreter(string configurationPath)
        {
            this.configurationPath = configurationPath;
            this._configuration = Toml.ReadString<Configuration>(configurationPath);
            Console.WriteLine(this._configuration);
        }

        public Configuration GetConfiguration()
        {
            return this._configuration;
        }
    }
}
