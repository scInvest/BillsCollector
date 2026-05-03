using BiedronkaParser.BiedronkaImport.Dto;
using System.Text.Json;

namespace DataSrouce
{
    public class ParagonFileReader
    {
        private readonly string _directoryPath;

        JsonSerializerOptions options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        public ParagonFileReader(string directoryPath)
        {
            _directoryPath = directoryPath;
        }

        public async Task<ReceiptDto> ReadFiles(string path)
        {
            var json = await File.ReadAllTextAsync(path);
            var dto = JsonSerializer.Deserialize<ReceiptDto>(json, options);
            return dto;
        }

        public async Task<IEnumerable<ReceiptDto>> ReadFiles(IEnumerable<string> paths)
        {
            var tasks = paths.Select(path => ReadFiles(path));
            var results = await Task.WhenAll(tasks);
            return results.Where(r => r != null);
        }


        public List<string> GetParagonFiles()
        {

            if (!Directory.Exists(_directoryPath))
            {
                throw new DirectoryNotFoundException($"Directory not found: {_directoryPath}");
            }

            var allFiles = Directory.GetFiles(_directoryPath, "*.json")
                .Where(f => Path.GetFileName(f).Contains("paragon", StringComparison.OrdinalIgnoreCase))
                .ToList();

            var uniqueFiles = new List<string>();

            foreach (var file in allFiles)
            {
                var fileName = Path.GetFileName(file);

                // Check if file has duplicate pattern like (1), (2), etc.
                if (!HasDuplicatePattern(fileName))
                {
                    uniqueFiles.Add(file);
                }
            }

            return uniqueFiles.OrderBy(f => f).ToList();
        }

        private bool HasDuplicatePattern(string fileName)
        {
            // Check for pattern like " (1)", " (2)", etc. before the extension
            var nameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
            return System.Text.RegularExpressions.Regex.IsMatch(nameWithoutExtension, @"\s\(\d+\)$");
        }

        public List<T> ReadParagonFiles<T>()
        {
            var files = GetParagonFiles();
            var results = new List<T>();

            foreach (var file in files)
            {
                try
                {
                    var json = File.ReadAllText(file);
                    var obj = System.Text.Json.JsonSerializer.Deserialize<T>(json);
                    if (obj != null)
                    {
                        results.Add(obj);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error reading file {file}: {ex.Message}");
                }
            }

            return results;
        }
    }
}
