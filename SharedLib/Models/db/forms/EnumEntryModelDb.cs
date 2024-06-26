////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Перечисление
/// </summary>
public class EnumEntryModelDb : EntryConstructedModel
{
    /// <summary>
    /// Elements of enum
    /// </summary>
    public List<ElementOfEnumEntryModelDb>? ElementsOfEnumeration { get; set; }
}