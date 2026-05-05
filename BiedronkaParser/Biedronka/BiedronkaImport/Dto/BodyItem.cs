using System.Text.Json.Serialization;

namespace Integrations.Biedronka.BiedronkaImport.Dto
{
    [JsonConverter(typeof(BodyItemConverter))]
    public abstract class BodyItem { }
}
