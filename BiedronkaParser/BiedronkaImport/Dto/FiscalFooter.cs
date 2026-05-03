using System;

namespace BiedronkaParser.BiedronkaImport.Dto
{
    public class FiscalFooter : BodyItem
    {
        public int BillNumber { get; set; }
        public string UniqueNumber { get; set; }
        public string CashNumber { get; set; }
        public string Cashier { get; set; }
        public int CPS { get; set; }
        public DateTime Date { get; set; }
    }
}
