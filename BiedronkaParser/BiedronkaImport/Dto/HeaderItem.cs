using System.Text.Json.Serialization;

namespace BiedronkaParser.BiedronkaImport.Dto
{
    [JsonConverter(typeof(HeaderItemConverter))]
    public abstract class HeaderItem { }
}
