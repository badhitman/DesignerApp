////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Selected field
/// </summary>
public class SelectedFieldModel
{
    /// <inheritdoc/>
    public Type FieldType { get; set; } = default!;

    /// <inheritdoc/>
    public int FieldId { get; set; } = default!;

    /// <inheritdoc/>
    public string FieldName { get; set; } = default!;
}