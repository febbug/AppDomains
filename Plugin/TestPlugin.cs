using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plugin
{
    public class TestPlugin
    {

        public static string ReadFile(string filePath)
        {
            return File.ReadAllText(filePath);
        }
    }
}
