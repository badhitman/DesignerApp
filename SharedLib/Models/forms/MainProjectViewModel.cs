////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Текущий/основной проект
/// </summary>
public class MainProjectViewModel : EntryDescriptionSwitchableModel
{
    /// <inheritdoc/>
    public static MainProjectViewModel Build(ProjectConstructorModelDb sender)
    {
        return new()
        {
            Name = sender.Name,
            Description = sender.Description,
            Id = sender.Id,
            IsDisabled = sender.IsDisabled,
        };
    }

    /// <inheritdoc/>
    public void Reload(MainProjectViewModel other)
    {
        Name = other.Name;
        Description = other.Description;
        IsDisabled = other.IsDisabled;
        Id = other.Id;
    }
}