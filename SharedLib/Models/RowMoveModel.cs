////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Row Move
/// </summary>
public class RowMoveModel
{
    /// <summary>
    /// Direction
    /// </summary>
    public VerticalDirectionsEnum Direction { get; set; }

    /// <summary>
    /// Object Id
    /// </summary>
    public int ObjectId { get; set; }

    /// <summary>
    /// ContextName
    /// </summary>
    public required string? ContextName { get; set; }
}