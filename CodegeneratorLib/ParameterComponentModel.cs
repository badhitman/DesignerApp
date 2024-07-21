////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov
////////////////////////////////////////////////

namespace CodegeneratorLib;

/// <summary>
/// Параметр компонента
/// </summary>
public class ParameterComponentModel(string name, string type, string description) : ParameterModel(name, type, description)
{
    public string? Constraints { get; set; }
}