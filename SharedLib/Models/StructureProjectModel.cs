﻿////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

namespace SharedLib.Models;

/// <summary>
/// Структура/Состав проекта
/// </summary>
public class StructureProjectModel
{
    /// <summary>
    /// Перечисления проекта
    /// </summary>
    public required IEnumerable<EnumFitModel> Enums { get; set; }

    /// <summary>
    /// Документы проекта
    /// </summary>
    public required IEnumerable<DocumentFitModel> Documents { get; set; }

    /// <summary>
    /// Адаптер конвертации перечислений
    /// </summary>
    public IEnumerable<EnumDesignModelDB> EnumsProxyAdapter
    {
        set
        {
            Enums = value.Select(x => (EnumFitModel)x);
        }
    }
}