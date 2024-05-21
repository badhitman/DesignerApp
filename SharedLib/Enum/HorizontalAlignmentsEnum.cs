////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using System.ComponentModel;

namespace SharedLib;

/// <summary>
/// Выравнивание по горизонтали
/// </summary>
public enum HorizontalAlignmentsEnum
{
    /// <summary>
    /// По левому краю
    /// </summary>
    [Description("По левому краю")]
    Left,

    /// <summary>
    /// По центру
    /// </summary>
    [Description("По центру")]
    Center,

    /// <summary>
    /// По правому краю
    /// </summary>
    [Description("По правому краю")]
    Right
}