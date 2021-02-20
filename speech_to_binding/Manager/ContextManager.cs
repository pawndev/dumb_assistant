using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace speech_to_binding.Manager
{
    public class ContextManager
    {
        public WindowManager windowManager;
        public ConfigurationManager configManager;
        public ContextManager(WindowManager windowManager, ConfigurationManager configurationManager)
        {
            this.windowManager = windowManager;
            this.configManager = configurationManager;
        }
    }
}
