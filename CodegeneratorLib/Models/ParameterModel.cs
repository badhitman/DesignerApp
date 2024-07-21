////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

namespace CodegeneratorLib;

/// <summary>
/// Parameter payload
/// </summary>
public class ParameterModel(string type, string description)
{

    /// <summary>
    /// Type parameter
    /// </summary>
    public string Type { get; set; } = type;

    /// <summary>
    /// Description parameter
    /// </summary>
    public string Description { get; set; } = description;
}