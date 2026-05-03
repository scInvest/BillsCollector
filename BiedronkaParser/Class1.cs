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

    public static class TagsConsts
    {
        public const string Biedronka = "Biedronka";
    }

    class ISpendingCaseReciptV1 : ISpendingCase
    {
        public ISpendingCaseReciptV1(ReceiptDto receipt, int processingIndex)
        {
            Receipt = receipt;
            Date = ExtractDateFromReceipt(receipt);
            Id = new ReceiptIdV1Dto(receipt, processingIndex);
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

        public string UserFriendlyName => TagsConsts.Biedronka + " " + Date.ToString("yyyy-MM-dd");
        public string Name => TagsConsts.Biedronka;


        public bool IsRoot => true;

        public bool IsLeaf => false;

        public ReceiptDto Receipt { get; }

        public IReadOnlyList<ISpendingCase> Childs => throw new NotImplementedException();

        public ISpendingTags Tags => throw new NotImplementedException();

        public ISpendingId Id { get; }

        public ISpendingDecorations Decorations { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public ISpending Summary { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    }

    internal class ReceiptIdV1Dto : ISpendingId
    {
        private readonly Dictionary<string, string> _allIds = new Dictionary<string, string>();

        public ReceiptIdV1Dto(int processingIndex)
        {
            this.Index = processingIndex;
        }

        public ReceiptIdV1Dto(ReceiptDto receipt, int processingIndex) : this(processingIndex)
        {
            PopulateIdsFromReceipt(receipt);
        }

        public int Index { get; set; }

        public string ID => _allIds.TryGetValue("ReceiptId", out var id) ? id : $"Receipt_{Index}";

        public IReadOnlyDictionary<string, string> AllIds => _allIds;

        private void PopulateIdsFromReceipt(ReceiptDto receipt)
        {
            if (!string.IsNullOrEmpty(receipt.IDZ))
            {
                _allIds["ReceiptId"] = receipt.IDZ;
            }

            if (!string.IsNullOrEmpty(receipt.ProtoVersion))
            {
                _allIds["ProtocolVersion"] = receipt.ProtoVersion;
            }

            _allIds["DeviceType"] = receipt.DeviceType.ToString();

            if (!string.IsNullOrEmpty(receipt.Sign))
            {
                _allIds["Signature"] = receipt.Sign;
            }

            foreach (var header in receipt.Header)
            {
                if (header is HeaderDataItem headerData)
                {
                    if (!string.IsNullOrEmpty(headerData.Tin))
                    {
                        _allIds["TaxId"] = headerData.Tin;
                    }

                    _allIds["DocumentNumber"] = headerData.DocNumber.ToString();
                    _allIds["ControlPointSystem"] = headerData.CPS.ToString();
                    break;
                }
            }
        }
    }
}
