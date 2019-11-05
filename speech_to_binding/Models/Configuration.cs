using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace speech_to_binding.Models
{
    enum ActionType
    {
        SPEAK
    }
    class Configuration
    {
        public string title { get; set; }
        public List<ConfigurationMatch> match { get; set; }

        public override string ToString()
        {
            string matchesString = String.Join(", ", this.match);
            return $"{{ title: '{this.title}', match: [{matchesString}] }}";
        }
    }

    class ConfigurationMatch
    {
        public string name { get; set; }
        public string description { get; set; }
        public string phrase { get; set; }
        public List<ConfigurationMatchAction> actions { get; set; }

        public override string ToString()
        {
            string actionsString = String.Join(", ", this.actions);
            return $"{{ name: '{this.name}', description: '{this.description}', phrase: {this.phrase}, action: [{actionsString}] }}";
        }
    }

    class ConfigurationMatchAction
    {
        public string description { get; set; }
        public bool is_async { get; set; }
        public ActionType type { get; set; }
        public string message { get; set; }

        public override string ToString()
        {
            return $"{{ description: '{this.description}', is_async: '{this.is_async}', type: {this.type}, message: {this.message} }}";
        }
    }
}
