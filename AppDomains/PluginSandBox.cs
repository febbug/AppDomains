using Contracts;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
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
        private IPluginApplication plugin;

        //Anything we need to call from plugin is wrapped in sandbox's method
        public string ExecutePlugin(string filePath)
        {
            return plugin.ReadFile(filePath);
        }

        //did not find a way to pass params to sandbox's constructor, so having this method :(
        internal void InitPlugin(string pluginPath)
        {
            var catalogue = new DirectoryCatalog(pluginPath);
            var container = new CompositionContainer(catalogue);
            plugin =  container.GetExportedValue<IPluginApplication>();
        }
    }
}
