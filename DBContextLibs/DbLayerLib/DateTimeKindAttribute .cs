using System.Reflection;

namespace DbLayerLib;

/// <inheritdoc/>
[AttributeUsage(AttributeTargets.Property)]
public class DateTimeKindAttribute(DateTimeKind _kind) : Attribute
{
    /// <inheritdoc/>
    public DateTimeKind Kind
    {
        get { return _kind; }
    }

    /// <inheritdoc/>
    public static void Apply(object entity)
    {
        if (entity == null)
            return;

        IEnumerable<PropertyInfo> properties = entity.GetType().GetProperties()
            .Where(x => x.PropertyType == typeof(DateTime) || x.PropertyType == typeof(DateTime?));

        foreach (PropertyInfo property in properties)
        {
            DateTimeKindAttribute? attr = property.GetCustomAttribute<DateTimeKindAttribute>();
            if (attr == null)
                continue;

            DateTime? dt = property.PropertyType == typeof(DateTime?)
                ? (DateTime?)property.GetValue(entity)
                : (DateTime?)property.GetValue(entity);

            if (dt == null)
                continue;

            property.SetValue(entity, DateTime.SpecifyKind(dt.Value, attr.Kind));
        }
    }
}