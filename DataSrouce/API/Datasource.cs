using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DataSrouce.API
{
    internal class Datasource 
    {
        public Datasource()
        {
            var baseDir = AppDomain.CurrentDomain.BaseDirectory;
            var dataPath = Path.GetFullPath(Path.Combine(baseDir, @"..\..\..\DataSrouce\BierdronkaData\"));
            TestData = new ParagonFileReader(dataPath);
        }
        public ParagonFileReader TestData { get; } 
    }
}
