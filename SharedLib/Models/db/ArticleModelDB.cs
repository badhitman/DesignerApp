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

    /// <summary>
    /// Tags
    /// </summary>
    public List<ArticleTagModelDB>? Tags { get; set; }

    /// <summary>
    /// Update
    /// </summary>
    public void Update(EntryModel[] tags)
    {
        if (Tags is null)
            Tags = tags.Select(x => new ArticleTagModelDB() { Id = x.Id, Name = x.Name, NormalizedNameUpper = x.Name.ToUpper(), OwnerArticleId = Id }).ToList();
        else
        {
            EntryModel[] _items = tags
                .Where(x => !Tags.Any(y => y.Id == x.Id))
                .ToArray();
            if (_items.Length != 0)
                Tags.AddRange(_items.Select(x => new ArticleTagModelDB() { Id = x.Id, Name = x.Name, NormalizedNameUpper = x.Name.ToUpper(), OwnerArticleId = Id }));

            Tags.RemoveAll(x => !tags.Any(y => y.Id == x.Id));
        }
    }
}