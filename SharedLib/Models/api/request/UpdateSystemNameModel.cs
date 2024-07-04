namespace SharedLib;

/// <summary>
/// Установить системное имя сущности.
/// </summary>
/// <remarks>
/// Если установить null (или пустую строку), тогда значение удаляется
/// </remarks>
public class UpdateSystemNameModel
{
    /// <summary>
    /// Тип данных, для которого выполняется запрос: перечисление, элемент перечисления, документ, таб документа, форма и т.д.
    /// </summary>
    /// <remarks>
    /// Используется имя типа данных DbSet { nameof(MyType) } соответствующего объекта
    /// </remarks>
    public required string TypeData { get; set; }

    /// <summary>
    /// Если установить null (или пустую строку), тогда значение удаляется
    /// </summary>
    public string? SystemName { get; set; }

    /// <summary>
    /// Генератор кода
    /// </summary>
    public required int ManufactureId { get; set; }
}