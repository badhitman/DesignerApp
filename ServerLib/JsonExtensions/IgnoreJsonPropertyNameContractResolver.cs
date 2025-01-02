using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using System.Reflection;

namespace ServerLib;

/// <summary>
/// IgnoreJsonPropertyNameContractResolver
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
