using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Integrations.Biedronka.BiedronkaImport.Dto
{
    public class HeaderItemConverter : JsonConverter<HeaderItem>
    {
        public override HeaderItem Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using var doc = JsonDocument.ParseValue(ref reader);
            var root = doc.RootElement;

            if (root.TryGetProperty("image", out var v))
                return JsonSerializer.Deserialize<ImageHeader>(v.GetRawText(), options);

            if (root.TryGetProperty("headerText", out v))
                return JsonSerializer.Deserialize<HeaderTextItem>(v.GetRawText(), options);

            if (root.TryGetProperty("headerData", out v))
                return JsonSerializer.Deserialize<HeaderDataItem>(v.GetRawText(), options);

            throw new NotSupportedException();
        }

        public override void Write(Utf8JsonWriter writer, HeaderItem value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            switch (value)
            {
                case ImageHeader i:
                    writer.WritePropertyName("image");
                    JsonSerializer.Serialize(writer, i, options);
                    break;

                case HeaderTextItem t:
                    writer.WritePropertyName("headerText");
                    JsonSerializer.Serialize(writer, t, options);
                    break;

                case HeaderDataItem d:
                    writer.WritePropertyName("headerData");
                    JsonSerializer.Serialize(writer, d, options);
                    break;
            }

            writer.WriteEndObject();
        }
    }
}
