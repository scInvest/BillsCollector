using System.Text.Json.Serialization;

namespace Integrations.Biedronka.BiedronkaImport.Dto
{
    [JsonConverter(typeof(HeaderItemConverter))]
    public abstract class HeaderItem { }
}
