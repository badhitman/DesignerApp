namespace SharedLib;

/// <summary>
/// Элемент перечисления
/// </summary>
public class ElementOfEnumEntryModelDb : EntryConstructedModel
{
    /// <summary>
    /// Owner enumeration
    /// </summary>
    public EnumEntryModelDb? OwnerEnumeration {  get; set; }

    /// <summary>
    /// Owner enumeration Id
    /// </summary>
    public int OwnerEnumerationId { get; set; }
}