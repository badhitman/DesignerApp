////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Universal base
/// </summary>
public class UniversalBaseModel : EntrySwitchableUpdatedModel
{
    /// <inheritdoc/>
    public int ProjectId { get; set; }

    /// <summary>
    /// Сортировка
    /// </summary>
    public uint SortIndex { get; set; }

    /// <summary>
    /// Родитель (вышестоящий)
    /// </summary>
    public int? ParentId { get; set; }

    /// <inheritdoc/>
    public override string ToString()
    {
        return $"{Id} {Name} [{SortIndex}] /{ParentId}";
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        if (obj == null) return false;

        if (obj is UniversalBaseModel e)
            return 
                Name == e.Name && 
                Description == e.Description && 
                Id == e.Id && 
                e.SortIndex == SortIndex && 
                e.ParentId == ParentId && 
                e.ProjectId == ProjectId && 
                e.IsDisabled == IsDisabled;

        return false;
    }

    /// <inheritdoc/>
    public static bool operator ==(UniversalBaseModel? e1, UniversalBaseModel? e2)
        => (e1 is null && e2 is null) || (e1?.Id == e2?.Id && e1?.Name == e2?.Name && e1?.Description == e2?.Description && e1?.SortIndex == e2?.SortIndex && e1?.ParentId == e2?.ParentId && e1?.ProjectId == e2?.ProjectId && e1?.IsDisabled == e2?.IsDisabled);

    /// <inheritdoc/>
    public static bool operator !=(UniversalBaseModel? e1, UniversalBaseModel? e2)
        => (e1 is null && e2 is not null) || (e1 is not null && e2 is null) || e1?.Id != e2?.Id || e1?.Name != e2?.Name || e1?.Description != e2?.Description || e1?.SortIndex != e2?.SortIndex || e1?.ParentId != e2?.ParentId || e1?.ProjectId != e2?.ProjectId || e1?.IsDisabled != e2?.IsDisabled;

    /// <inheritdoc/>
    public override int GetHashCode()
        => $"{ProjectId}|{ParentId} {Id}/{SortIndex}/{IsDisabled} {Name} {Description}".GetHashCode();

    /// <inheritdoc/>
    public void Update(UniversalBaseModel sender)
    {
        ProjectId = sender.ProjectId;
        Name = sender.Name;
        Description = sender.Description;
        SortIndex = sender.SortIndex;
        ParentId = sender.ParentId;
        IsDisabled = sender.IsDisabled;
        Id = sender.Id;
    }
}