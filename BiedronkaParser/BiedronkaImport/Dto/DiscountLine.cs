namespace ClassLibrary1.BiedronkaImport.Dto
{
    public class DiscountLine : BodyItem
    {
        public int Base { get; set; }
        public int Value { get; set; }
        public bool IsDiscount { get; set; }
        public bool IsPercent { get; set; }
        public bool IsStorno { get; set; }
        public string VatId { get; set; }
    }
}
