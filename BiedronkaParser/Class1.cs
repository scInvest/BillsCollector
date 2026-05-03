using BiedronkaParser.BiedronkaImport.Dto;
using CostAnalizerApp.Interfaces;

namespace BiedronkaParser
{
    public class Class1
    {
        //public ISpendingCase ConvertToStandardFromat(ReceiptDto receipt)
        //{

        //}
    }


    class ISpendingCaseReciptV1 : ISpendingCase
    {
        public ISpendingCaseReciptV1(ReceiptDto receipt)
        {
            Receipt = receipt;
            Date = ExtractDateFromReceipt(receipt);
        }

        private DateTime ExtractDateFromReceipt(ReceiptDto receipt)
        {
            foreach (var header in receipt.Header)
            {
                if (header is HeaderDataItem headerData)
                {
                    return headerData.Date;
                }
            }
            throw new InvalidOperationException("Date not found in receipt headers");
        }

        public DateTime Date { get; }

        public string UserFriendlyName => throw new NotImplementedException();

        public bool IsRoot => true;

        public bool IsLeaf => false;

        public ReceiptDto Receipt { get; }

        public IReadOnlyList<ISpendingCase> Childs => throw new NotImplementedException();

        public SpendingTags Tags => throw new NotImplementedException();

        public ISpendingId Id => throw new NotImplementedException();

        public ISpendingDecorations Decorations { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public ISpending Summary { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    }
}
