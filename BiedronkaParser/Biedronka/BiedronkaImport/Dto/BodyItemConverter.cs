using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Integrations.Biedronka.BiedronkaImport.Dto
{
    public class BodyItemConverter : JsonConverter<BodyItem>
    {
        public override BodyItem Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using var doc = JsonDocument.ParseValue(ref reader);
            var root = doc.RootElement;

            if (root.TryGetProperty("sellLine", out var v))
                return JsonSerializer.Deserialize<SellLine>(v.GetRawText(), options);

            if (root.TryGetProperty("discountLine", out v))
                return JsonSerializer.Deserialize<DiscountLine>(v.GetRawText(), options);

            if (root.TryGetProperty("discountSummary", out v))
                return JsonSerializer.Deserialize<DiscountSummary>(v.GetRawText(), options);

            if (root.TryGetProperty("vatSummary", out v))
                return JsonSerializer.Deserialize<VatSummary>(v.GetRawText(), options);

            if (root.TryGetProperty("sumInCurrency", out v))
                return JsonSerializer.Deserialize<SumInCurrency>(v.GetRawText(), options);

            if (root.TryGetProperty("section", out v))
                return JsonSerializer.Deserialize<Section>(v.GetRawText(), options);

            if (root.TryGetProperty("payment", out v))
                return JsonSerializer.Deserialize<Payment>(v.GetRawText(), options);

            if (root.TryGetProperty("fiscalFooter", out v))
                return JsonSerializer.Deserialize<FiscalFooter>(v.GetRawText(), options);

            if (root.TryGetProperty("addLine", out v))
                return JsonSerializer.Deserialize<AddLine>(v.GetRawText(), options);

            if (root.TryGetProperty("barcode", out v))
                return JsonSerializer.Deserialize<Barcode>(v.GetRawText(), options);

            if (root.TryGetProperty("sysNumber", out v))
                return JsonSerializer.Deserialize<SysNumber>(v.GetRawText(), options);

            if (root.TryGetProperty("pack", out v))
                return JsonSerializer.Deserialize<Pack>(v.GetRawText(), options);

            throw new NotSupportedException($"Unknown body item: {root}");
        }

        public override void Write(Utf8JsonWriter writer, BodyItem value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            switch (value)
            {
                case SellLine s:
                    writer.WritePropertyName("sellLine");
                    JsonSerializer.Serialize(writer, s, options);
                    break;

                case DiscountLine d:
                    writer.WritePropertyName("discountLine");
                    JsonSerializer.Serialize(writer, d, options);
                    break;

                case DiscountSummary d:
                    writer.WritePropertyName("discountSummary");
                    JsonSerializer.Serialize(writer, d, options);
                    break;

                case VatSummary v:
                    writer.WritePropertyName("vatSummary");
                    JsonSerializer.Serialize(writer, v, options);
                    break;

                case SumInCurrency s:
                    writer.WritePropertyName("sumInCurrency");
                    JsonSerializer.Serialize(writer, s, options);
                    break;

                case Section s:
                    writer.WritePropertyName("section");
                    JsonSerializer.Serialize(writer, s, options);
                    break;

                case Payment p:
                    writer.WritePropertyName("payment");
                    JsonSerializer.Serialize(writer, p, options);
                    break;

                case FiscalFooter f:
                    writer.WritePropertyName("fiscalFooter");
                    JsonSerializer.Serialize(writer, f, options);
                    break;

                case AddLine a:
                    writer.WritePropertyName("addLine");
                    JsonSerializer.Serialize(writer, a, options);
                    break;

                case Barcode b:
                    writer.WritePropertyName("barcode");
                    JsonSerializer.Serialize(writer, b, options);
                    break;

                case SysNumber s:
                    writer.WritePropertyName("sysNumber");
                    JsonSerializer.Serialize(writer, s, options);
                    break;

                case Pack p:
                    writer.WritePropertyName("pack");
                    JsonSerializer.Serialize(writer, p, options);
                    break;

                default:
                    throw new NotSupportedException();
            }

            writer.WriteEndObject();
        }
    }
}
