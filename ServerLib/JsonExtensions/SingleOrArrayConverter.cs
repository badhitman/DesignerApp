using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace ServerLib;

/// <summary>
/// SingleOrArrayConverter
/// </summary>
public class SingleOrArrayConverter<T> : JsonConverter
{
    /// <summary>
    /// CanConvert
    /// </summary>
    public override bool CanConvert(Type objectType) => objectType == typeof(List<T>);
    /// <summary>
    /// 
    /// </summary>
    public override bool CanWrite => false;
    /// <summary>
    /// 
    /// </summary>
    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        => throw new NotSupportedException();

    /// <summary>
    /// 
    /// </summary>
    public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
    {
        JToken token = JToken.Load(reader);

        if (token.Type == JTokenType.Array)
        {
            return token.ToObject<List<T>>();
        }

        return new List<T?> { token.ToObject<T>() };
    }
}