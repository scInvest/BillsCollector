namespace BiedronkaParser.BiedronkaImport.Dto
{
    public class Pack : BodyItem
    {
        public string Name { get; set; }
        public int Price { get; set; }
        public string Quantity { get; set; }
        public int Total { get; set; }
        public bool IsNegative { get; set; }
    }
}
