using CostAnalizerApp.Interfaces;
using Integrations.Biedronka.BiedronkaImport.Dto;

namespace Integrations.Biedronka
{
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
            return new ValueFactory().CreateCount(count);
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
}
