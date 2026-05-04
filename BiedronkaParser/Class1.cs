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

    public static class TagsConsts
    {
        public const string Biedronka = "Biedronka";
        public const string Leaf = "Leaf";
        public const string TopLevel = "TopLevel";
        public const string Kaucaja = "Kaucaja";
    }

    class ISpendingCaseReciptV1 : ISpendingCase
    {
        public ISpendingCaseReciptV1(ReceiptDto receipt, int processingIndex)
        {
            Receipt = receipt;
            Date = ExtractDateFromReceipt(receipt);
            Id = new ReceiptIdV1Dto(receipt, processingIndex);
            Tags = CreateTags();
            Summary = ExtractSummaryFromBody(receipt);
        }

        private ISpendingTags CreateTags()
        {
            var tags = new SpendingTags(this);
            tags.Tags.Add(TagsConsts.Biedronka);
            return tags;
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

        private ISpending ExtractSummaryFromBody(ReceiptDto receipt)
        {
            var discount = 0.0;
            var total = 0.0;

            if (receipt.Body != null)
            {
                foreach (var item in receipt.Body)
                {
                    if (item is DiscountSummary discountSummary)
                    {
                        discount = discountSummary.Discounts / 100.0;
                    }
                    else if (item is SumInCurrency sumInCurrency)
                    {
                        total = sumInCurrency.FiscalTotal / 100.0;
                    }
                }
            }

            return new SpendingImp
            {
                Discount = discount,
                Total = total,
                Cost = total + discount,
                Quantity = new ValueFatory().CreateNull(),
            };
        }

        public DateTime Date { get; }

        public string UserFriendlyName => TagsConsts.Biedronka + " " + Date.ToString("yyyy-MM-dd");
        public string Name => TagsConsts.Biedronka;
        public bool IsRoot => true;

        public bool IsLeaf => false;

        public ReceiptDto Receipt { get; }

        public ISpendingCase? Parent { get; }

        public IReadOnlyList<ISpendingCase> Childs => throw new NotImplementedException();

        public ISpendingTags Tags { get; }

        public ISpendingId Id { get; }

        public ISpendingDecorations Decorations { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public ISpending Summary { get; }

    }

    public class SpendingImp : ISpending
    {
        public double Cost { get; set; }
        public double Discount { get; set; }
        public double Total { get; set; }

        public IValue Quantity { get; set; }

    }
    class SpendingTags : ISpendingTags
    {
        public SpendingTags(ISpendingCase parent)
        {
            if (parent != null)
            {
                if (parent.IsLeaf)
                {
                    Tags.Add(TagsConsts.Leaf);
                }
                if (parent.IsRoot)
                {
                    Tags.Add(TagsConsts.TopLevel);
                }
            }
        }

        public List<string> Tags { get; } = new List<string>();
        public List<string> GroupingTags { get; } = new List<string>();

        IReadOnlyList<string> ISpendingTags.Tags => Tags;
        IReadOnlyList<string> ISpendingTags.GroupingTags => GroupingTags;
    }
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
