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
    class Program : MarshalByRefObject
    {

        const string pathToPlugin = @"..\..\..\Plugin\bin\Debug";
        const string pluginAssembly = @"Plugin";
        const string pluginClass = "Plugin.TestPlugin";
        const string entryPoint = "ReadFile";
        //private static Object[] parameters = { @"C:\Users\febbu\source\repos\AppDomains\Plugin\bin\Debug\Plugin.txt" };
        private static Object[] parameters = { @"c:\temp\Plugin.txt" };




        static void Main(string[] args)
        {
            AppDomainSetup adSetup = new AppDomainSetup();
            adSetup.ApplicationBase = Path.GetFullPath(pathToPlugin);

            //Setting the permissions for the AppDomain. We give the permission to execute and to
            //read/discover the location where the untrusted code is loaded.  
            PermissionSet permSet = new PermissionSet(PermissionState.None);
            permSet.AddPermission(new SecurityPermission(SecurityPermissionFlag.Execution));

            FileIOPermission fp = new FileIOPermission(
                    FileIOPermissionAccess.Read |
                        FileIOPermissionAccess.PathDiscovery,
                    @"C:\Users\febbu\source\repos\AppDomains\Plugin\bin\Debug");


            permSet.AddPermission(fp);



            StrongName fullTrustAssembly = typeof(Program).Assembly.Evidence.GetHostEvidence<StrongName>();

            AppDomain newDomain = AppDomain.CreateDomain("Sandbox", null, adSetup, permSet, fullTrustAssembly);

            ObjectHandle handle = Activator.CreateInstanceFrom(
            newDomain, typeof(Program).Assembly.ManifestModule.FullyQualifiedName,
            typeof(Program).FullName
            );

            //untrusted code.  
            Program newDomainInstance = (Program)handle.Unwrap();
            Console.WriteLine(newDomainInstance.ExecuteUntrustedCode(pluginAssembly, pluginClass, entryPoint, parameters));


            Console.ReadKey();

        }

        public string ExecuteUntrustedCode(string assemblyName, string typeName, string entryPoint, Object[] parameters)
        {

            string retVal = "";
            //Load the MethodInfo for a method in the new Assembly. This might be a method you know, or
            //you can use Assembly.EntryPoint to get to the main function in an executable.  
            MethodInfo target = Assembly.Load(assemblyName).GetType(typeName).GetMethod(entryPoint);
            try
            {
                //Now invoke the method.  
                retVal =  (string)target.Invoke(null, parameters);
            }
            catch (Exception ex)
            {
                // When we print informations from a SecurityException extra information can be printed if we are
                //calling it with a full-trust stack.  
                new PermissionSet(PermissionState.Unrestricted).Assert();
                Console.WriteLine("SecurityException caught:\n{0}", ex.ToString());
                CodeAccessPermission.RevertAssert();
                Console.ReadLine();
            }

            return retVal;
        }
    }
}
