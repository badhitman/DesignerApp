////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace CodegeneratorLib;

/// <summary>
/// Parameter payload
/// </summary>
public class ParameterModel(string name, string type, string description)
{
    /// <summary>
    /// Имя параметра
    /// </summary>
    public string Name => name;

    /// <summary>
    /// Type parameter
    /// </summary>
    public string Type => type;

    /// <summary>
    /// Description parameter
    /// </summary>
    public string Description => description;
}