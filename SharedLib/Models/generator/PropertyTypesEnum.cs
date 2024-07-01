////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

namespace SharedLib.Models;

/// <summary>
/// Типы полей дизайн-моделей
/// </summary>
public enum PropertyTypesEnum
{
    /// <summary>
    /// Целое число
    /// </summary>
    Int,

    /// <summary>
    /// Число с точкой
    /// </summary>
    Decimal,

    /// <summary>
    /// Строка
    /// </summary>
    String,

    /// <summary>
    /// Булево
    /// </summary>
    Bool,

    /// <summary>
    /// ДатаВремя
    /// </summary>
    DateTime,

    /// <summary>
    /// Простое статическое перечисление
    /// </summary>
    SimpleEnum,

    /// <summary>
    /// Документ
    /// </summary>
    Document
}
