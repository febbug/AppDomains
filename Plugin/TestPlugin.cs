using Contracts;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plugin
{
    [Export(typeof(IPluginApplication))]
    public class TestPlugin: IPluginApplication 
    {

        public  string ReadFile(string filePath)
        {
            return File.ReadAllText(filePath);
        }
    }
}
