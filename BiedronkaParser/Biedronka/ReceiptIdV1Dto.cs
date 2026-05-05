using CostAnalizerApp.Interfaces;
using Integrations.Biedronka.BiedronkaImport.Dto;

namespace Integrations.Biedronka
{
    internal class ReceiptIdV1Dto : ISpendingId
    {
        private readonly Dictionary<string, string> _allIds = new Dictionary<string, string>();
        private readonly string _id;

        public ReceiptIdV1Dto(int processingIndex)
        {
            this.Index = processingIndex;
            _id = $"Receipt_{Index}";
        }

        public ReceiptIdV1Dto(ReceiptDto receipt, int processingIndex) : this(processingIndex)
        {
            Receipt = receipt;
            PopulateIdsFromReceipt(receipt);
            _id = _allIds.TryGetValue("ReceiptId", out var id) ? id : $"Receipt_{Index}";
        }

        public ReceiptDto Receipt { get; }
        public int Index { get; init; }

        public string ID => _id;

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
