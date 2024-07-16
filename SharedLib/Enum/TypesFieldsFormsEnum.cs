////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using System.ComponentModel;

namespace SharedLib;

/// <summary>
/// Типы данных полей форм
/// </summary>
public enum TypesFieldsFormsEnum
{
    /// <summary>
    /// Текст
    /// </summary>
    [Description("Текст")]
    Text = 10,

    /// <summary>
    /// Секрет
    /// </summary>
    [Description("Секрет")]
    Password = 20,

    /// <summary>
    /// Целочисленное значение
    /// </summary>
    [Description("Целое число")]
    Int = 30,

    /// <summary>
    /// Дробное число
    /// </summary>
    [Description("Дробное число")]
    Double = 40,

    /// <summary>
    /// Булево
    /// </summary>
    [Description("Чекбокс")]
    Bool = 50,

    /// <summary>
    /// Дата
    /// </summary>
    [Description("Дата")]
    Date = 60,

    /// <summary>
    /// Время
    /// </summary>
    [Description("Время")]
    Time = 70,

    /// <summary>
    /// Дата + время
    /// </summary>
    [Description("Дата+Время")]
    DateTime = 80,

    /// <summary>
    /// Калькуляция  (double; программное;)
    /// </summary>
    [Description("Калькуляция")]
    ProgramCalculationDouble  = 1100,

    /// <summary>
    /// Генератор (rest-api)
    /// </summary>
    [Description("Генератор")]
    Generator = 1200,
}