using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace ServerLib;

/// <summary>
/// StrippingBigData
/// </summary>
public class StrippingBigData : JsonConverter
{
    private readonly int Size;

    /// <summary>
    /// 
    /// </summary>
    public StrippingBigData(int Size) => this.Size = Size;

    /// <summary>
    /// 
    /// </summary>
    public override bool CanConvert(Type objectType) => objectType == typeof(string) || objectType == typeof(byte[]);
    /// <summary>
    /// 
    /// </summary>
    public override bool CanRead => false;
    /// <summary>
    /// 
    /// </summary>
    public override object ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        => throw new NotSupportedException();

    /// <summary>
    /// 
    /// </summary>
    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
    {
        if (value is null)
            return;

        JToken token = JToken.FromObject(value);
        var size = 0;

        switch (token.Type)
        {
            case JTokenType.Bytes:
                size = token.Value<byte[]>()?.Length ?? -1;
                break;
            case JTokenType.String:
                size = token.Value<string>()?.Length ?? -1;
                break;
        }

        if (size > Size)
        {
            token = $"### {size / 1024.0:F1}KB ###";
        }

        token.WriteTo(writer);
    }
}
