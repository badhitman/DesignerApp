////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;

namespace SharedLib;

/// <summary>
/// Рубрики для обращений
/// </summary>
[Index(nameof(SortIndex), nameof(ParentRubricId), IsUnique = true)]
public class RubricBaseModel : EntryDescriptionSwitchableModel
{
    /// <inheritdoc/>
    public int ProjectId { get; set; }

    /// <summary>
    /// Сортировка
    /// </summary>
    public uint SortIndex { get; set; }

    /// <summary>
    /// Владелец (вышестоящая рубрика)
    /// </summary>
    public int? ParentRubricId { get; set; }

    /// <inheritdoc/>
    public override string ToString()
    {
        return $"{Id} {Name} [{SortIndex}] /{ParentRubricId}";
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        if (obj == null) return false;

        if (obj is RubricBaseModel e)
            return Name == e.Name && Description == e.Description && Id == e.Id && e.SortIndex == SortIndex && e.ParentRubricId == ParentRubricId && e.ProjectId == ProjectId && e.IsDisabled == IsDisabled;

        return false;
    }

    /// <inheritdoc/>
    public static bool operator ==(RubricBaseModel? e1, RubricBaseModel? e2)
        => (e1 is null && e2 is null) || (e1?.Id == e2?.Id && e1?.Name == e2?.Name && e1?.Description == e2?.Description && e1?.SortIndex == e2?.SortIndex && e1?.ParentRubricId == e2?.ParentRubricId && e1?.ProjectId == e2?.ProjectId && e1?.IsDisabled == e2?.IsDisabled);

    /// <inheritdoc/>
    public static bool operator !=(RubricBaseModel? e1, RubricBaseModel? e2)
        => (e1 is null && e2 is not null) || (e1 is not null && e2 is null) || e1?.Id != e2?.Id || e1?.Name != e2?.Name || e1?.Description != e2?.Description || e1?.SortIndex != e2?.SortIndex || e1?.ParentRubricId != e2?.ParentRubricId || e1?.ProjectId != e2?.ProjectId || e1?.IsDisabled != e2?.IsDisabled;

    /// <inheritdoc/>
    public override int GetHashCode()
        => $"{ProjectId}|{ParentRubricId} {Id}/{SortIndex}/{IsDisabled} {Name} {Description}".GetHashCode();

    /// <inheritdoc/>
    public void Update(RubricBaseModel sender)
    {
        ProjectId = sender.ProjectId;
        Name = sender.Name;
        Description = sender.Description;
        SortIndex = sender.SortIndex;
        ParentRubricId = sender.ParentRubricId;
        IsDisabled = sender.IsDisabled;
        Id = sender.Id;
    }
}