////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

namespace CodegeneratorLib;

/// <summary>
/// Базовая модель
/// </summary>
public abstract class BaseFormFitModel : SortableFitModel
{
    /// <summary>
    /// CSS класс формы
    /// </summary>
    public string? Css { get; set; }
}