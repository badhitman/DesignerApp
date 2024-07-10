////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

namespace SharedLib.Models;

/// <summary>
/// Базовая модель
/// </summary>
public abstract class BaseRequiredmFormFitModel : BaseFormFitModel
{
    /// <summary>
    /// Обязательность для заполнения
    /// </summary>
    public bool Required { get; set; }
}