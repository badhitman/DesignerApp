using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using System.Reflection;

namespace ServerLib;

/// <summary>
/// 
/// </summary>
public class SingleOrArrayConverter<T> : JsonConverter
{
    /// <summary>
    /// 
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

/// <summary>
/// 
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

/// <summary>
/// 
/// </summary>
public class DateTimeConverter : IsoDateTimeConverter
{
    /// <summary>
    /// 
    /// </summary>
    public DateTimeConverter() => DateTimeFormat = "dd.MM.yyyy";
}

/// <summary>
/// 
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

/// <summary>
/// 
/// </summary>
public class IgnoreJsonPropertyNameContractResolver : DefaultContractResolver
{
    /// <summary>
    /// 
    /// </summary>
    override protected JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
    {
        var property = base.CreateProperty(member, memberSerialization);
        property.PropertyName = member.Name;
        return property;
    }
}

/// <summary>
/// 
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