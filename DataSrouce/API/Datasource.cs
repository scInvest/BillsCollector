using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DataSrouce.API
{
    public class Datasource 
    {
        public Datasource()
        {
            var dataPath = FindDataDirectory();
            ParagonTestData = new ParagonFileReader(dataPath);
        }
        public ParagonFileReader ParagonTestData { get; }

        private static string FindDataDirectory()
        {
            var currentDir = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);

            while (currentDir != null)
            {
                var dataPath = Path.Combine(currentDir.FullName, "DataSrouce", "BierdronkaData");
                if (Directory.Exists(dataPath))
                {
                    return dataPath;
                }
                currentDir = currentDir.Parent;
            }

            throw new DirectoryNotFoundException(
                "Could not find DataSrouce\\BierdronkaData directory. " +
                "Searched from: " + AppDomain.CurrentDomain.BaseDirectory);
        }
    }
}
