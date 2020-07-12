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

        const string pathToPlugin = @"c:\temp\Plugin";

        private static string filePath =@"c:\temp\Plugin.txt" ; //this one causes security exception
        //private static string filePath = @"c:\temp\Plugin\Plugin.txt";

        static void Main(string[] args)
        {
            PluginSandBox pluginSandBox = GetPluginSandBox();
            Console.WriteLine(pluginSandBox.ExecutePlugin(filePath));
            Console.ReadKey();

        }

        private static PluginSandBox GetPluginSandBox()
        {
            AppDomainSetup adSetup = new AppDomainSetup();
            var sandBoxPath = Path.GetFullPath(pathToPlugin);
            adSetup.ApplicationBase = sandBoxPath;

            //Setting the permissions for the AppDomain. We give the permission to execute and to
            //read/discover the location where the untrusted code is loaded.  
            PermissionSet permSet = new PermissionSet(PermissionState.None);
            permSet.AddPermission(new SecurityPermission(SecurityPermissionFlag.Execution));

            FileIOPermission fp = new FileIOPermission(
                    FileIOPermissionAccess.Read |
                        FileIOPermissionAccess.PathDiscovery,sandBoxPath); //read permissions set only to plugin directory


            permSet.AddPermission(fp);
            StrongName fullTrustAssembly = typeof(PluginSandBox).Assembly.Evidence.GetHostEvidence<StrongName>();
            AppDomain sandBoxDomain = AppDomain.CreateDomain("Sandbox", null, adSetup, permSet, fullTrustAssembly);
            
            //TODO this way we could pass the path right to the constructor
            //var pSandBox = (PluginSandBox)sandBoxDomain.CreateInstanceAndUnwrap(typeof(PluginSandBox).Assembly.ManifestModule.FullyQualifiedName,
            //typeof(PluginSandBox).FullName, new[] { sandBoxPath });

            ObjectHandle handle = Activator.CreateInstanceFrom(sandBoxDomain, typeof(PluginSandBox).Assembly.ManifestModule.FullyQualifiedName,
            typeof(PluginSandBox).FullName);
            PluginSandBox pSandBox = (PluginSandBox)handle.Unwrap();
            pSandBox.InitPlugin(sandBoxPath);
            return pSandBox;
        }
    }
}
