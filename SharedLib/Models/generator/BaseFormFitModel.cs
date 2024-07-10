////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

namespace SharedLib.Models;

/// <summary>
/// Базовая модель
/// </summary>
public abstract class BaseFormFitModel : SortableFitModel
{
    /// <summary>
    /// Подсказка
    /// </summary>
    public string? Hint { get; set; }

    /// <summary>
    /// CSS класс формы
    /// </summary>
    public string? Css { get; set; }
}