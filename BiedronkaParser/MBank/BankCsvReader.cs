using Integrations.MBank;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

public class BankCsvReader
{
    public static List<BankTransaction> Load(string path)
    {
        var result = new List<BankTransaction>();

        var text = Encoding.UTF8.GetString(File.ReadAllBytes(path));

        var lines = text
            .Split('\n')
            .Select(l => l.Trim())
            .Where(l => !string.IsNullOrWhiteSpace(l))
            .ToList();

        // 🔍 znajdź nagłówek
        int startIndex = lines.FindIndex(l => l.Contains("Data operacji"));

        if (startIndex >= 0)
            startIndex++;
        else
            startIndex = lines.FindIndex(IsDataLine);

        for (int i = startIndex; i < lines.Count; i++)
        {
            var line = lines[i];

            var parts = SplitCsv(line);

            if (parts.Count < 5)
                continue;

            try
            {
                var transaction = new BankTransaction
                {
                    OperationDate = DateTime.Parse(parts[0]),
                    Description = parts[1],
                    Account = parts[2],
                    Category = parts[3],
                    Amount = ParseAmount(parts[4])
                };

                result.Add(transaction);
            }
            catch
            {
                // opcjonalnie log
            }
        }

        return result;
    }

    // 🔥 poprawne dzielenie CSV z cudzysłowami
    private static List<string> SplitCsv(string line)
    {
        var result = new List<string>();
        var current = new StringBuilder();
        bool inQuotes = false;

        foreach (char c in line)
        {
            if (c == '"')
            {
                inQuotes = !inQuotes;
                continue;
            }

            if (c == ';' && !inQuotes)
            {
                result.Add(current.ToString().Trim());
                current.Clear();
            }
            else
            {
                current.Append(c);
            }
        }

        result.Add(current.ToString().Trim());

        return result;
    }

    private static decimal ParseAmount(string input)
    {
        // "-1 062,00 PLN" → -1062.00
        input = input
            .Replace("PLN", "")
            .Replace(" ", "")
            .Replace(",", ".");

        return decimal.Parse(input, CultureInfo.InvariantCulture);
    }

    private static bool IsDataLine(string line)
    {
        return line.Length > 10 && char.IsDigit(line[0]);
    }
}