////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Текущий/основной проект
/// </summary>
public class MainProjectViewModel : EntryDescriptionModel
{
    /// <inheritdoc/>
    public void Reload(MainProjectViewModel other)
    {
        Name = other.Name;
        Description = other.Description;
        IsDeleted = other.IsDeleted;
        Id = other.Id;
    }
}