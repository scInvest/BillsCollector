namespace Integrations.Biedronka.BiedronkaImport.Dto
{
    public class Payment : BodyItem
    {
        public string Type { get; set; }
        public int Amount { get; set; }
        public string Name { get; set; }
        public string Currency { get; set; }
    }
}
