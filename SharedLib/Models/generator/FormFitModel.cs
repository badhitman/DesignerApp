﻿////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

namespace SharedLib.Models;

/// <summary>
/// Форма
/// </summary>
public class FormFitModel : BaseFormFitModel
{
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