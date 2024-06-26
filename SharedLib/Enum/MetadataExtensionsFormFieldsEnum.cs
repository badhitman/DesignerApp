////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using System.ComponentModel;

namespace SharedLib;

/// <summary>
/// Опции расширения типов [полей форм]
/// </summary>
public enum MetadataExtensionsFormFieldsEnum
{
    /// <summary>
    /// Многострочный
    /// </summary>
    [Description("Многострочный")]
    IsMultiline = 30,

    /// <summary>
    /// Параметер
    /// </summary>
    [Description("Параметер")]
    Parameter = 40,

    /// <summary>
    /// Вид параметра
    /// </summary>
    [Description("Вид параметра")]
    Descriptor = 50,

    /// <summary>
    /// Текст-заполнитель
    /// </summary>
    [Description("Текст-заполнитель")]
    Placeholder = 60
}