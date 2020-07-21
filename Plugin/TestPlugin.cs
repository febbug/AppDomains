using Contracts;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Plugin
{
    [Export(typeof(IPluginApplication))]
    public class TestPlugin : IPluginApplication
    {

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr MessageBox(int hWnd, String text,
                     String caption, uint type);

        public string ReadFile(string filePath)
        {
            //MessageBox(0, "Hello From Plugin", "Platform Invoke", 0);
            return File.ReadAllText(filePath);
        }
    }
}
