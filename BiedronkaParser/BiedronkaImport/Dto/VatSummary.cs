using System.Collections.Generic;

namespace ClassLibrary1.BiedronkaImport.Dto
{
    public class VatSummary : BodyItem
    {
        public string Currency { get; set; }
        public List<VatRate> VatRatesSummary { get; set; }
    }
}
