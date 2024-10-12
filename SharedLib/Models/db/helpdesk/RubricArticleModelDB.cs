﻿////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Рубрики для статей
/// </summary>
public class RubricArticleModelDB : RubricArticleMiddleModel
{
    /// <summary>
    /// Обращения в рубрике
    /// </summary>
    public List<RubricArticleJoinModelDB>? ArticlesJoins { get; set; }

    /// <summary>
    /// Владелец (вышестоящая рубрика)
    /// </summary>
    public RubricArticleModelDB? ParentRubric { get; set; }


    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        if (obj == null) return false;

        if (obj is RubricArticleModelDB e)
            return Name == e.Name && Description == e.Description && Id == e.Id && e.SortIndex == SortIndex && e.ParentRubricId == ParentRubricId && e.ProjectId == ProjectId;

        return false;
    }

    /// <inheritdoc/>
    public static bool operator ==(RubricArticleModelDB? e1, RubricArticleModelDB? e2)
        =>
        (e1 is null && e2 is null) ||
        (e1?.Id == e2?.Id && e1?.Name == e2?.Name && e1?.Description == e2?.Description && e1?.SortIndex == e2?.SortIndex && e1?.ParentRubricId == e2?.ParentRubricId && e1?.ProjectId == e2?.ProjectId);

    /// <inheritdoc/>
    public static bool operator !=(RubricArticleModelDB? e1, RubricArticleModelDB? e2)
        =>
        (e1 is null && e2 is not null) ||
        (e1 is not null && e2 is null) ||
        e1?.Id != e2?.Id ||
        e1?.Name != e2?.Name ||
        e1?.Description != e2?.Description ||
        e1?.SortIndex != e2?.SortIndex ||
        e1?.ParentRubricId != e2?.ParentRubricId ||
        e1?.ProjectId != e2?.ProjectId;

    /// <inheritdoc/>
    public override int GetHashCode()
    => $"{ParentRubricId} {SortIndex} {Name} {Id} {Description}".GetHashCode();
}