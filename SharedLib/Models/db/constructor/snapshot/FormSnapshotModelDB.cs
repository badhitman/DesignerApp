////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Form of document snapshot
/// </summary>
public class FormSnapshotModelDB : SortableSystemSnapshotModelDB
{
    /// <summary>
    ///Tab [PARENT OWNER]
    /// </summary>
    public TabSnapshotModelDB? Owner { get; set; }

    /// <inheritdoc/>
    public required FieldSnapshotModelDB[] SimpleFields { get; set; }

    /// <inheritdoc/>
    public required FieldAkaDirectorySnapshotModelDB[] DirectoryFields { get; set; }

    /// <inheritdoc/>
    public Dictionary<uint, BaseFieldModel> AllFields
    {
        get
        {
            Dictionary<uint, BaseFieldModel> res = [];

            SimpleFields
                .Cast<BaseFieldModel>()
                .Union(DirectoryFields.Cast<BaseFieldModel>())
                .OrderBy(x => x.SortIndex)
                .ToList()
                .ForEach(x => res.Add(x.SortIndex, x));

            return res;
        }
    }
}