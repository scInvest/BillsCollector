using BiedronkaParser.BiedronkaImport.Dto;
using CostAnalizerApp.Interfaces;

namespace BiedronkaParser
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
