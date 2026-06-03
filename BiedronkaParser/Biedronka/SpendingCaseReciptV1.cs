using CostAnalizerApp.Interfaces;
using Integrations.Biedronka.BiedronkaImport.Dto;

namespace Integrations.Biedronka
{
    class SpendingCaseReciptV1 : ISpendingCase
    {
        private readonly List<ISpendingCase> _childs = new List<ISpendingCase>();
        public SpendingCaseReciptV1(ReceiptDto receipt, int processingIndex)
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
                Quantity = new ValueFactory().CreateNull(),
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
