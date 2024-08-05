////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
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
    public required IEnumerable<FieldSnapshotModelDB> SimpleFields { get; set; }

    /// <inheritdoc/>
    public required IEnumerable<FieldAkaDirectorySnapshotModelDB> DirectoryFields { get; set; }

    /// <inheritdoc/>
    public Dictionary<int, BaseFieldModel> AllFields
    {
        get
        {
            Dictionary<int, BaseFieldModel> res = [];

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