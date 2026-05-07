namespace CostAnalizerApp.Interfaces
{
    public class ValueFactory
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
        public IValue CreateCount(double amount)
        {
            return new ValueImp()
            {
                Amount = amount,
                Type = "zakup",
                Unit = "sztuka",
            };
        }
    }
}
