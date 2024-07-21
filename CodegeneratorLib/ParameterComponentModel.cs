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
    /// Режим параметра: обязательный или Nullable
    /// </summary>
    public ParameterModes? ParameterMode { get; set; }

    /* public string? Constraints { get; set; } */
}