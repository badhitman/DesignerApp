namespace SharedLib;

/// <summary>
/// Перечисление
/// </summary>
public class EnumEntryModelDb : SystemEntryModel
{
    /// <summary>
    /// Elements of enum
    /// </summary>
    public List<ElementOfEnumEntryModelDb>? ElementsOfEnumeration { get; set; }
}