using System.Text.Json.Serialization;

namespace ClassLibrary1.BiedronkaImport.Dto
{
    [JsonConverter(typeof(BodyItemConverter))]
    public abstract class BodyItem { }
}
