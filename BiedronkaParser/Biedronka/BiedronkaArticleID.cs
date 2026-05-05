using BiedronkaParser.BiedronkaImport.Dto;
using CostAnalizerApp.Interfaces;

namespace BiedronkaParser
{
    class BiedronkaArticleID : ISpendingId
    {
        private readonly Dictionary<string, string> _allIds = new Dictionary<string, string>();
        private readonly string _id;

        public BiedronkaArticleID(SellLine sellLine, int index)
        {
            Index = index;

            // Populate secondary IDs
            if (!string.IsNullOrEmpty(sellLine.Name))
            {
                _allIds["ArticleName"] = sellLine.Name;
            }

            if (!string.IsNullOrEmpty(sellLine.VatId))
            {
                _allIds["VatId"] = sellLine.VatId;
            }

            _allIds["Price"] = sellLine.Price.ToString();
            _allIds["Quantity"] = sellLine.Quantity;
            _allIds["IsStorno"] = sellLine.IsStorno.ToString();

            // Create primary ID from index and name
            _id = $"Article_{index}_{sellLine.Name?.Replace(" ", "_") ?? "Item"}";
        }

        public int Index { get; }

        public string ID => _id;

        public IReadOnlyDictionary<string, string> AllIds => _allIds;
    }
}
