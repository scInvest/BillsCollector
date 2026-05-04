namespace CostAnalizerApp.Interfaces
{
    /// <summary>
    /// Reprezentuje pojedyczy wydatek. Uproszczona bazowka
    /// </summary>
    public interface ISpending
    {
        public double Cost { get; }
        public double Discount { get; }
        public double Total { get; }
        
        public IValue Quantity { get; }

    }

    class ValueImp : IValue
    {
        public string Type { get; set; }
        public double Amount { get; set; }
        public string Unit { get; set; }
    }

    public class ValueFatory
    {
        public IValue CreateNull()
        {
            return new ValueImp()
            {
                Amount = 0,
                Type = "NULL",
                Unit = "",
            }; 
        }
    }

    public interface IValue
    {
        public string Type { get; set; }
        public double Amount { get; }
        public string Unit { get;  }
    }
}
