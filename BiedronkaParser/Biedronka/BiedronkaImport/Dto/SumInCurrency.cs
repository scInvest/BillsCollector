namespace Integrations.Biedronka.BiedronkaImport.Dto
{
    public class SumInCurrency : BodyItem
    {
        public int FiscalTotal { get; set; }
        public int TotalWithPacks { get; set; }
        public string Currency { get; set; }
        public bool PrintBig { get; set; }
        public bool Printable { get; set; }
    }
}
