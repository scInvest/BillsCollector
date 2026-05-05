using CostAnalizerApp.Interfaces;

namespace BiedronkaParser
{
    public class SpendingImp : ISpending
    {
        public double Cost { get; set; }
        public double Discount { get; set; }
        public double Total { get; set; }

        public IValue Quantity { get; set; }

    }
}
