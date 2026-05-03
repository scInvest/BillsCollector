using System;

namespace ClassLibrary1.BiedronkaImport.Dto
{
    public class HeaderDataItem : HeaderItem
    {
        public string Tin { get; set; }
        public int DocNumber { get; set; }
        public DateTime Date { get; set; }
        public int CPS { get; set; }
    }
}
