////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

namespace CodegeneratorLib;

/// <summary>
/// Форма
/// </summary>
public class FormFitModel : BaseFormFitModel
{
    /// <summary>
    /// Имя связи формы с табом
    /// </summary>
    public string? JoinName { get; set; }

    /// <summary>
    /// Табличная часть
    /// </summary>
    public bool IsTable { get; set; }

    /// <summary>
    /// Простые поля
    /// </summary>
    public FieldFitModel[]? SimpleFields { get; set; }

    /// <summary>
    /// Поля типа: справочник/список/перечисление
    /// </summary>
    public FieldAkaDirectoryFitModel[]? FieldsAtDirectories { get; set; }
}