using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting;
using System.Security;
using System.Security.Permissions;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace AppDomains
{
    class Program
    {

        const string pathToPlugin = @"..\..\..\Plugin\bin\Debug";

        private static string filePath =@"c:\temp\Plugin.txt" ; //this one causes security exception
        //private static string filePath = @"C:\Users\febbu\source\repos\AppDomains\Plugin\bin\Debug\Plugin.txt";

        static void Main(string[] args)
        {
            PluginSandBox pluginSandBox = GetPluginSandBox();
            Console.WriteLine(pluginSandBox.ExecutePlugin(filePath));
            Console.ReadKey();

        }

        private static PluginSandBox GetPluginSandBox()
        {
            AppDomainSetup adSetup = new AppDomainSetup();
            var pluginPath = Path.GetFullPath(pathToPlugin);
            adSetup.ApplicationBase = pluginPath;

            //Setting the permissions for the AppDomain. We give the permission to execute and to
            //read/discover the location where the untrusted code is loaded.  
            PermissionSet permSet = new PermissionSet(PermissionState.None);
            permSet.AddPermission(new SecurityPermission(SecurityPermissionFlag.Execution));

            FileIOPermission fp = new FileIOPermission(
                    FileIOPermissionAccess.Read |
                        FileIOPermissionAccess.PathDiscovery,pluginPath); //read permissions set only to plugin directory


            permSet.AddPermission(fp);
            StrongName fullTrustAssembly = typeof(PluginSandBox).Assembly.Evidence.GetHostEvidence<StrongName>();
            AppDomain newDomain = AppDomain.CreateDomain("Sandbox", null, adSetup, permSet, fullTrustAssembly);
            ObjectHandle handle = Activator.CreateInstanceFrom(newDomain, typeof(PluginSandBox).Assembly.ManifestModule.FullyQualifiedName,
            typeof(PluginSandBox).FullName);

            //untrusted code.  
            PluginSandBox pLoader = (PluginSandBox)handle.Unwrap();
            return pLoader;
        }
    }
}
