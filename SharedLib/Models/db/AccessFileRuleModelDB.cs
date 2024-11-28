////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using System.ComponentModel.DataAnnotations;

namespace SharedLib;

/// <summary>
/// Правило доступа к файлу
/// </summary>
public class AccessFileRuleModelDB
{
    /// <summary>
    /// Идентификатор/Key
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// StoreFile
    /// </summary>
    public StorageFileModelDB? StoreFile { get; set; }

    /// <summary>
    /// StoreFile
    /// </summary>
    public int StoreFileId { get; set; }

    /// <summary>
    /// AccessRuleType
    /// </summary>
    public FileAccessRulesTypesEnum AccessRuleType { get; set; }

    /// <summary>
    /// Option
    /// </summary>
    public required string Option { get; set; }
}
