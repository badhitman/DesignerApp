////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Базовая модель
/// </summary>
public abstract class BaseRequiredFormFitModel : BaseFormFitModel
{
    /// <summary>
    /// Подсказка
    /// </summary>
    public string? Hint { get; set; }

    /// <summary>
    /// Обязательность для заполнения
    /// </summary>
    public bool Required { get; set; }
}