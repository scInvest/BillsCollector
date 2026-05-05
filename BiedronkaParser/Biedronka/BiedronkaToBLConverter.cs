using CostAnalizerApp.Interfaces;
using Integrations.Biedronka.BiedronkaImport.Dto;

namespace Integrations.Biedronka
{
    public class BiedronkaToBLConverter
    {
        public ISpendingCase ConvertToStandardFromat(ReceiptDto receipt)
        {
            var result = new ISpendingCaseReciptV1(receipt, 0);

            return result;
        }
    }
}
