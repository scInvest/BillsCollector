using Integrations.Allegro.Import;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
namespace Integrations.Allegro.Import
{
    public class AllegroCsvReader
    {
        public static List<AllegroOrder> Load(string path)
        {
            var result = new List<AllegroOrder>();

            // wczytaj jako bajty → unikamy problemów encodingu
            var bytes = File.ReadAllBytes(path);
            var text = Encoding.UTF8.GetString(bytes);

            var lines = text.Split('\n');

            for (int i = 1; i < lines.Length; i++) // pomijamy nagłówek
            {
                var line = lines[i].Trim();
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                var parts = line.Split(';');

                var order = new AllegroOrder
                {
                    OfferId = long.Parse(parts[0]),
                    Title = Fix(parts[1].Trim('\'')),
                    PurchaseDate = DateTime.Parse(parts[2].Trim('\'')),
                    Quantity = int.Parse(parts[3]),
                    OriginalPrice = decimal.Parse(parts[4], CultureInfo.InvariantCulture),
                    SellerLogin = parts[5].Trim('\'')
                };

                result.Add(order);
            }

            return result;
        }

        // naprawa krzaków typu ÅÄ
        private static string Fix(string input)
        {
            var bytes = Encoding.GetEncoding("Windows-1250").GetBytes(input);
            return Encoding.UTF8.GetString(bytes);
        }
    }
}