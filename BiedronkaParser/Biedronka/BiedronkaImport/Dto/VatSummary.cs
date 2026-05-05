using System.Collections.Generic;

namespace Integrations.Biedronka.BiedronkaImport.Dto
{
    public class VatSummary : BodyItem
    {
        public string Currency { get; set; }
        public List<VatRate> VatRatesSummary { get; set; }
    }
}
