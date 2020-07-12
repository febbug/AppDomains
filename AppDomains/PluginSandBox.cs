using Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace AppDomains
{
    public class PluginSandBox : MarshalByRefObject
    {
        const string pluginAssembly = @"Plugin";
        const string pluginClass = "Plugin.TestPlugin";
        const string entryPoint = "ReadFile";
        private readonly IPluginApplication plugin;

        public PluginSandBox()
        {
            //TODO load this via MEF 🤞
            this.plugin = AppDomain.CurrentDomain.CreateInstanceAndUnwrap(pluginAssembly, pluginClass) as IPluginApplication;

        }
        public string ExecutePlugin(string filePath)
        {
            return plugin.ReadFile(filePath);
        }

    }
}
