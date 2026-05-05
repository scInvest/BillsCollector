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

    class SpendingArticleFromRecitp : ISpendingCase
    {
        public SpendingArticleFromRecitp(ISpendingCase parent, SellLine sellLine, int index)
                        : this(parent, sellLine, null, index)
        {
        }

        public SpendingArticleFromRecitp(ISpendingCase parent, SellLine sellLine, DiscountLine? discountLine, int index)
        {
            Summary = ExtractSummary(sellLine, discountLine);
            this.Decorations = new DecorationDummyImp();
            this.Id = new BiedronkaArticleID(sellLine, 0);
            this.Node = new SpendingCaseNodeDummy
            {
                Childs = new List<ISpendingCase>(),
                Parent = parent
            };
            this.Name =  sellLine.Name;
            this.UserFriendlyName =  sellLine.Name;
            this.Date = parent.Date;
            this.Tags = CreateTags();

        }

        private ISpending ExtractSummary(SellLine sellLine, DiscountLine? discountLine)
        {
            var total = sellLine.Total / 100.0;
            var discount = discountLine?.Value / 100.0 ?? 0.0;
            var quantity = ExtractQuantity(sellLine);
            return CreateSpending(total, discount, quantity);
        }

        private IValue ExtractQuantity(SellLine sellLine)
        {
            var count = double.Parse(sellLine.Quantity);
            return new ValueFatory().CreateCount(count);
        }

        private ISpending CreateSpending(double total, double discount, IValue quantity)
        {
            return new SpendingImp
            {
                Discount = discount,
                Total = total,
                Cost = total - discount,
                Quantity = quantity,
            };
        }

        private ISpendingTags CreateTags()
        {
            var tags = new SpendingTags(this);
            tags.Tags.Add(TagsConsts.Product);
            return tags;
        }

        public DateTime Date { get; }

        public string UserFriendlyName { get; }

        public string Name { get; }

        public ISpendingTags Tags { get; }

        public ISpendingId Id { get; }

        public ISpendingDecorations Decorations { get; }

        public ISpending Summary { get; }

        public ISpendingCaseNode Node { get; }
    }


    class ISpendingCaseReciptV1 : ISpendingCase
    {
        private readonly List<ISpendingCase> _childs = new List<ISpendingCase>();
        public ISpendingCaseReciptV1(ReceiptDto receipt, int processingIndex)
        {
            Receipt = receipt;
            Date = ExtractDateFromReceipt(receipt);
            Id = new ReceiptIdV1Dto(receipt, processingIndex);
            Summary = ExtractSummaryFromBody(receipt);
            this.Decorations = new DecorationDummyImp();
            this.Node = new SpendingCaseNodeDummy
            {
                Childs = _childs,
                Parent = null
            };
            PopulateChildsFromReceipt(receipt);
            Tags = CreateTags();
        }

        private void PopulateChildsFromReceipt(ReceiptDto receipt)
        {
            if (receipt.Body == null)
                return;

            int articleIndex = 0;
            for (int i = 0; i < receipt.Body.Count; i++)
            {
                var item = receipt.Body[i];

                if (item is SellLine sellLine)
                {
                    DiscountLine? nextDiscount = null;

                    // Check if next item is a discount
                    if (i + 1 < receipt.Body.Count && receipt.Body[i + 1] is DiscountLine discountLine)
                    {
                        nextDiscount = discountLine;
                    }

                    var article = nextDiscount != null
                        ? new SpendingArticleFromRecitp(this, sellLine, nextDiscount, articleIndex)
                        : new SpendingArticleFromRecitp(this, sellLine, articleIndex);

                    _childs.Add(article);
                    articleIndex++;
                }
            }
        }

        private ISpendingTags CreateTags()
        {
            var tags = new SpendingTags(this);
            tags.Tags.Add(TagsConsts.Biedronka);
            return tags;
        }

        private DateTime ExtractDateFromReceipt(ReceiptDto receipt)
        {
            foreach (var header in receipt.Header)
            {
                if (header is HeaderDataItem headerData)
                {
                    return headerData.Date;
                }
            }
            throw new InvalidOperationException("Date not found in receipt headers");
        }

        private ISpending ExtractSummaryFromBody(ReceiptDto receipt)
        {
            var discount = 0.0;
            var total = 0.0;

            if (receipt.Body != null)
            {
                foreach (var item in receipt.Body)
                {
                    if (item is DiscountSummary discountSummary)
                    {
                        discount = discountSummary.Discounts / 100.0;
                    }
                    else if (item is SumInCurrency sumInCurrency)
                    {
                        total = sumInCurrency.FiscalTotal / 100.0;
                    }
                }
            }

            return new SpendingImp
            {
                Discount = discount,
                Total = total,
                Cost = total + discount,
                Quantity = new ValueFatory().CreateNull(),
            };
        }
        public DateTime Date { get; }
        public string UserFriendlyName => TagsConsts.Biedronka + " " + Date.ToString("yyyy-MM-dd");
        public string Name => TagsConsts.Biedronka;
        public ReceiptDto Receipt { get; }
        public ISpendingTags Tags { get; }
        public ISpendingId Id { get; }
        public ISpendingDecorations Decorations { get; }
        public ISpending Summary { get; }

        public ISpendingCaseNode Node { get; }
    }
}

class SpendingCaseNodeDummy : ISpendingCaseNode
{
    public bool IsRoot => Parent == null;

    public bool IsLeaf => Childs?.Any() != true;

    public ISpendingCase? Parent { get; set; }

    public IReadOnlyList<ISpendingCase> Childs { get; set; }
}
