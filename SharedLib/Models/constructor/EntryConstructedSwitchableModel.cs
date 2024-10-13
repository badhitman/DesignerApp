////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// System entry
/// </summary>
public class EntryConstructedSwitchableModel : EntryDescriptionSwitchableModel
{
    /// <summary>
    /// Project
    /// </summary>
    public ProjectModelDb? Project { get; set; }

    /// <summary>
    /// Project
    /// </summary>
    public required int ProjectId { get; set; }

    /// <inheritdoc/>
    public static EntryConstructedSwitchableModel Build(EntrySwitchableModel sender, int projectId)
    {
        return new()
        {
            Id = sender.Id,
            Name = sender.Name,
            ProjectId = projectId,
            IsDisabled = sender.IsDisabled,
        };
    }
}