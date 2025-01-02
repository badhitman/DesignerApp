using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace ServerLib;

/// <summary>
/// JsonAsStringConverter
/// </summary>
public class JsonAsStringConverter : JsonConverter
{
    /// <summary>
    /// 
    /// </summary>
    public override bool CanConvert(Type objectType) => true;

    /// <summary>
    /// 
    /// </summary>
    public override object ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        => JToken.Load(reader).ToString();

    /// <summary>
    /// 
    /// </summary>
    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
    {
        if (value is null)
            return;
        JToken t = JToken.FromObject(value);
        serializer.Serialize(writer, t.ToString());
    }
}
