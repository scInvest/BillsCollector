namespace Integrations.Biedronka.BiedronkaImport.Dto
{
    public class SellLine : BodyItem
    {
        public string Name { get; set; }
        public string VatId { get; set; }
        public int Price { get; set; }
        public int Total { get; set; }
        public string Quantity { get; set; }
        public bool IsStorno { get; set; }
    }
}
