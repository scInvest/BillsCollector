using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;

namespace DataSrouce.API
{
    public class SessionData
    {

        public void AddBiedronkaData(string key, object value)
        {
            BiedronkaData[key] = value;
        }
        public DataCollection BiedronkaData { get; set; } = new DataCollection();
    }


    public class DataCollection : IReadOnlyDictionary<string, object>
    {
        private readonly Dictionary<string, object> _data = new Dictionary<string, object>();

        public object this[string key]
        {
            get => _data[key];
            set => _data[key] = value;
        }

        public IEnumerable<string> Keys => _data.Keys;

        public IEnumerable<object> Values => _data.Values;

        public int Count => _data.Count;

        public bool ContainsKey(string key)
        {
            return _data.ContainsKey(key);
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return _data.GetEnumerator();
        }

        public bool TryGetValue(string key, [MaybeNullWhen(false)] out object value)
        {
            return _data.TryGetValue(key, out value);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
    public class DatasourceAPI
    {
        public DatasourceAPI()
        {
            var dataPath = FindDataDirectory();
            ParagonTestData = new ParagonFileReader(dataPath);
        }

        public SessionData Session { get; set; }

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
