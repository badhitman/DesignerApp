////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;

namespace SharedLib;

/// <summary>
/// BaseFieldModel
/// </summary>
[Index(nameof(OwnerId), nameof(SortIndex), IsUnique = true)]
public class BaseFieldModel : SortableSystemSnapshotModelDB
{
    /// <inheritdoc/>
    public FormSnapshotModelDB? Owner { get; set; }
}