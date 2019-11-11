using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using speech_to_binding.Models;
using speech_to_binding.Services;

namespace speech_to_binding.Manager
{
    public class ConfigurationManager
    {
        private Dictionary<string, Configuration> _configurations = new Dictionary<string, Configuration>();

        public ConfigurationManager() { }

        public void Add(string configurationName, Configuration config)
        {
            this._configurations.Add(configurationName, config);
        }

        public Configuration Get(string configurationName)
        {
            return this._configurations[configurationName];
        }

        public List<ConfigurationMatch> GetAllMatch()
        {
            List<ConfigurationMatch> matches = new List<ConfigurationMatch>();
            foreach (KeyValuePair<string, Configuration> config in this._configurations)
            {
                matches = matches.Concat(config.Value.match).ToList();
            }

            return matches;
        }
    }
}
