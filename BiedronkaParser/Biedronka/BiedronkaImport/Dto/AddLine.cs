namespace Integrations.Biedronka.BiedronkaImport.Dto
{
    public class AddLine : BodyItem
    {
        public int Id { get; set; }
        public string Data { get; set; }
        public int Width { get; set; }
        public int CPS { get; set; }
    }
}
