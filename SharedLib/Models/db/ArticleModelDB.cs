////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace SharedLib;

/// <summary>
/// StorageArticleModelDB
/// </summary>
[Index(nameof(CreatedAtUTC)), Index(nameof(LastUpdatedAtUTC)), Index(nameof(AuthorIdentityId))]
public class ArticleModelDB : EntryDescriptionModel
{
    /// <inheritdoc/>
    public int ProjectId { get; set; }

    /// <summary>
    /// Сортировка
    /// </summary>
    public uint SortIndex { get; set; }

    /// <summary>
    /// ToUpper
    /// </summary>
    public string? NormalizedNameUpper { get; set; }

    /// <summary>
    /// IsDisabled
    /// </summary>
    public bool IsDisabled { get; set; }

    /// <summary>
    /// Рубрики
    /// </summary>
    public List<RubricArticleJoinModelDB>? RubricsJoins { get; set; }

    /// <summary>
    /// CreatedAtUTC
    /// </summary>
    public DateTime CreatedAtUTC { get; set; }

    /// <summary>
    /// CreatedAtUTC
    /// </summary>
    public DateTime? LastUpdatedAtUTC { get; set; }

    /// <summary>
    /// AuthorIdentityId
    /// </summary>
    public required string AuthorIdentityId { get; set; }
}