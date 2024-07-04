namespace SharedLib;

/// <summary>
/// SystemNameEntry
/// </summary>
public class SystemNameEntryModel
{
    /// <summary>
    /// Идентификатор владельца
    /// </summary>
    public required int TypeDataId { get; set; }

    /// <summary>
    /// Тип данных, для которого выполняется запрос: перечисление, элемент перечисления, документ, таб документа, форма и т.д.
    /// </summary>
    /// <remarks>
    /// Используется имя типа данных DbSet { nameof(MyType) } соответствующего объекта
    /// </remarks>
    public required string TypeDataName { get; set; }

    /// <summary>
    /// Если установить null (или пустую строку), тогда значение удаляется
    /// </summary>
    public string? SystemName { get; set; }
}
