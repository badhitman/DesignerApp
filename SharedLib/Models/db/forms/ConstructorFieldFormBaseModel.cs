using Newtonsoft.Json;

namespace SharedLib;

/// <summary>
/// Поле формы (простой тип)
/// </summary>
public class ConstructorFieldFormBaseModel : ConstructorFieldFormBaseLowModel
{
    /// <summary>
    /// Тип данных поля
    /// </summary>
    public TypesFieldsFormsEnum TypeField { get; set; }

    /// <summary>
    /// Метаданне типа значения (параметры/ограничения)
    /// </summary>
    public string? MetadataValueType { get; set; }

    /// <summary>
    /// Получить/распарсить метаданные
    /// </summary>
    public Dictionary<MetadataExtensionsFormFieldsEnum, object?> MetadataValueTypeGet()
    {
        if (string.IsNullOrWhiteSpace(MetadataValueType))
            return new();

        try
        {
            return JsonConvert.DeserializeObject<Dictionary<MetadataExtensionsFormFieldsEnum, object?>>(MetadataValueType) ?? new();
        }
        catch
        {
            MetadataValueType = null;
            return new();
        }
    }

    /// <summary>
    /// Удалить конкретное значение конкретного свойства из метаданных
    /// </summary>
    public void UnsetValueOfMetadata(MetadataExtensionsFormFieldsEnum prop_index)
    {
        Dictionary<MetadataExtensionsFormFieldsEnum, object?> dd = MetadataValueTypeGet();
        lock (locker)
        {
            dd.Remove(prop_index);
            MetadataValueType = JsonConvert.SerializeObject(dd);
        }
    }

    /// <summary>
    /// Установить конкретное значение конкретного свойства метаданных
    /// </summary>
    public void SetValueOfMetadata(MetadataExtensionsFormFieldsEnum prop_index, object? prop_value)
    {
        Dictionary<MetadataExtensionsFormFieldsEnum, object?> dd = MetadataValueTypeGet();
        lock (locker)
        {
            if (dd.ContainsKey(prop_index))
                dd[prop_index] = prop_value;
            else
                dd.Add(prop_index, prop_value);

            MetadataValueType = JsonConvert.SerializeObject(dd);
        }
    }

    /// <summary>
    /// Получить значение метаданных (если существует). Если свойства нет: вернёт default_value
    /// </summary>
    public object? TryGetValueOfMetadata(MetadataExtensionsFormFieldsEnum prop_index, object? default_value = null)
    {
        Dictionary<MetadataExtensionsFormFieldsEnum, object?> dd = MetadataValueTypeGet();
        if (dd.TryGetValue(prop_index, out object? prop_value))
            return prop_value;

        return default_value;
    }

    /// <summary>
    /// Сравнить метаданные (вместе с их значениями)
    /// </summary>
    public bool EqualMetadata(Dictionary<MetadataExtensionsFormFieldsEnum, object> other_dd)
    {
        Dictionary<MetadataExtensionsFormFieldsEnum, object?> dd = MetadataValueTypeGet();

        if (other_dd.Any(x => !dd.ContainsKey(x.Key)) || dd.Any(x => !other_dd.ContainsKey(x.Key)))
            return false;

        foreach (KeyValuePair<MetadataExtensionsFormFieldsEnum, object> d in other_dd)
            if (dd[d.Key]?.ToString() != d.Value.ToString())
                return false;

        return true;
    }

    static readonly object locker = new();
}