﻿////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Перечисление (лёгкая модель)
/// </summary>
public class EnumFitModel : BaseFitModel
{
    /// <summary>
    /// Элементы/состав перечисления
    /// </summary>
    public required SortableFitModel[] EnumItems { get; set; }
}