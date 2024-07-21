////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov
////////////////////////////////////////////////

namespace CodegeneratorLib;

/// <summary>
/// Параметр компонента
/// </summary>
public class ParameterComponent
{
    /// <summary>
    /// Тип параметра
    /// </summary>
    public required string TypeParameter { get; set; }

    /// <summary>
    /// Имя параметра
    /// </summary>
    public required string NameParameter { get; set; }

    /// <summary>
    /// Параметр получает значение из маршрута компонента-страницы
    /// </summary>
    public bool FromRoute { get; set; }

    /// <summary>
    /// Режим параметра: обязательный или Nullable
    /// </summary>
    public ParameterModes? ParameterMode { get; set; }
}