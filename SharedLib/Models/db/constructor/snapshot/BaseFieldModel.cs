////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;

namespace SharedLib;

/// <summary>
/// BaseFieldModel
/// </summary>
[Index(nameof(OwnerId), nameof(SortIndex), IsUnique = true)]
public abstract class BaseFieldModel : SortableSystemSnapshotModelDB
{
    /// <inheritdoc/>
    public FormSnapshotModelDB? Owner { get; set; }
}