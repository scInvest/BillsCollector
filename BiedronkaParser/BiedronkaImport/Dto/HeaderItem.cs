using System.Text.Json.Serialization;

namespace ClassLibrary1.BiedronkaImport.Dto
{
    [JsonConverter(typeof(HeaderItemConverter))]
    public abstract class HeaderItem { }
}
