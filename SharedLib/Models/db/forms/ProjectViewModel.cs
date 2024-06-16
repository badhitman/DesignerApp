
namespace SharedLib;

/// <inheritdoc/>
public class ProjectViewModel : EntryDescriptionModel
{
    /// <inheritdoc/>
    public static ProjectViewModel Build(ProjectViewModel other)
    {
        return new()
        {
            Name = other.Name,
            SystemName = other.SystemName,
            Description = other.Description,
            Id = other.Id,
            IsDeleted = other.IsDeleted,
            Members = other.Members,
        };
    }

    /// <inheritdoc/>
    public required string SystemName { get; set; }

    /// <summary>
    /// Участники проекта
    /// </summary>
    public EntryAltModel[]? Members { get; set; }

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        if (obj is ProjectViewModel other)
            return Id == other.Id && Name == other.Name && Description == other.Description && IsDeleted == other.IsDeleted;

        return false;
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        return $"{Id} {Name} {Description} {IsDeleted}".GetHashCode();
    }

    /// <inheritdoc/>
    public void Reload(ProjectViewModel other)
    {
        Name = other.Name;
        SystemName = other.SystemName;
        Description = other.Description;
        Id = other.Id;
        IsDeleted = other.IsDeleted;
        Members = other.Members;
    }
}