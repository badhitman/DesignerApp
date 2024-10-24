////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// SelectFilesRequestModel
/// </summary>
public class SelectFilesRequestModel : SelectRequestBaseModel
{
    /// <summary>
    /// Приложение
    /// </summary>
    public required string[] ApplicationsNames { get; set; }

    /// <summary>
    /// Имя
    /// </summary>
    public required string PropertyName { get; set; }

    /// <summary>
    /// Префикс
    /// </summary>
    public string? PrefixPropertyName { get; set; }

    /// <summary>
    /// Идентификатор [PK] владельца объекта
    /// </summary>
    public int? OwnerPrimaryKey { get; set; }
}