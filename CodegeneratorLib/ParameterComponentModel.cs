////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov
////////////////////////////////////////////////

namespace CodegeneratorLib;

/// <summary>
/// Параметр компонента
/// </summary>
public class ParameterComponentModel(string name, string type, string description) : ParameterModel(name, type, description)
{
    /// <summary>
    /// Параметр является каскадным
    /// </summary>
    public bool IsCascading { get; set; }

    /// <summary>
    /// Режим работы параметра: обязательный, nullable
    /// </summary>
    public ParameterModes? ParameterMode { get; set; }
}