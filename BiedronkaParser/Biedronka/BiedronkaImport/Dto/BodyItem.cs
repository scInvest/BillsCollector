using System.Text.Json.Serialization;

namespace BiedronkaParser.BiedronkaImport.Dto
{
    [JsonConverter(typeof(BodyItemConverter))]
    public abstract class BodyItem { }
}
