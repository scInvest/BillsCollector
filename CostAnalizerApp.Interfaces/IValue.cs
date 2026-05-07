namespace CostAnalizerApp.Interfaces
{
    public interface IValue
    {
        public string Type { get; set; }
        public double Amount { get; }
        public string Unit { get; }
    }
}
