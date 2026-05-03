namespace CostAnalizerApp.Interfaces
{
    /// <summary>
    /// Reprezentuje pojedyczy wydatek. Uproszczona bazowka
    /// </summary>
    public interface ISpending
    {
        public string Name { get; }
        public double Cost { get; }
        public double Discount { get; }
        public double Total { get; }

    }
}
