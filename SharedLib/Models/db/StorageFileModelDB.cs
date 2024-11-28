////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;

namespace SharedLib;

/// <inheritdoc/>
[Index(nameof(ReferrerMain)),Index(nameof(NormalizedFileNameUpper))]
public class StorageFileModelDB : StorageFileMiddleModel
{
    /// <summary>
    /// Нормализованное название (UPPER CASE)
    /// </summary>
    public string? NormalizedFileNameUpper { get; set; }

    /// <summary>
    /// Размер файла
    /// </summary>
    public long FileLength { get; set; }

    /// <summary>
    /// ContentType
    /// </summary>
    public string? ContentType { get; set; }

    /// <summary>
    /// Referrer (main/init)
    /// </summary>
    public string? ReferrerMain { get; set; }

    /// <summary>
    /// Правила доступа к файлу
    /// </summary>
    public List<AccessFileRuleModelDB>? AccessRules { get; set; }
}