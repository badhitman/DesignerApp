using Newtonsoft.Json;

namespace ServerLib;

/// <summary>
/// JsonEmptyStringToNullConverter
/// </summary>
public class JsonEmptyStringToNullConverter : JsonConverter
{
    /// <summary>
    /// 
    /// </summary>
    public override bool CanConvert(Type objectType) => objectType == typeof(string);

    /// <summary>
    /// 
    /// </summary>
    public override object ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        => throw new NotSupportedException();

    /// <summary>
    /// 
    /// </summary>
    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        => writer.WriteValue(((string?)value)?.Length == 0 ? default : value);
}