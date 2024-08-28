////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Field of document snapshot
/// </summary>
public class FieldSnapshotModelDB : BaseFieldModel
{

    /// <inheritdoc/>
    public required TypesFieldsFormsEnum TypeField { get; set; }
}