////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov
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
    /// Указывает, что компоненты маршрутизации могут предоставлять значение параметра из текущей строки запроса URL-адреса.
    /// Они также могут предоставлять дополнительные значения, если изменится строка запроса URL-адреса.
    /// </summary>
    public bool IsSupplyParameterFromQuery { get; set; }

    /// <summary>
    /// Режим работы параметра: обязательный, nullable
    /// </summary>
    public ParameterModes? ParameterMode { get; set; }
}