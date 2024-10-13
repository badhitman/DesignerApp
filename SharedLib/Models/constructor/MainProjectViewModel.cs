////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Текущий/основной проект
/// </summary>
public class MainProjectViewModel : EntryDescriptionSwitchableModel
{
    /// <summary>
    /// Владелец проекта
    /// </summary>
    public required string OwnerUserId { get; set; }

    /// <inheritdoc/>
    public void Reload(MainProjectViewModel other)
    {
        Name = other.Name;
        Description = other.Description;
        IsDisabled = other.IsDisabled;
        Id = other.Id;
        OwnerUserId = other.OwnerUserId;
    }

    /// <inheritdoc/>
    public static MainProjectViewModel Build(ProjectModelDb sender)
    {
        return new()
        {
            Name = sender.Name,
            Description = sender.Description,
            Id = sender.Id,
            IsDisabled = sender.IsDisabled,
            OwnerUserId = sender.OwnerUserId,
        };
    }
}