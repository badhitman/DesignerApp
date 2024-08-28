namespace SharedLib;

/// <summary>
/// Установить системное имя сущности.
/// </summary>
/// <remarks>
/// Если установить null (или пустую строку), тогда значение удаляется
/// </remarks>
public class UpdateSystemNameModel : SystemNameEntryModel
{
    /// <summary>
    /// Генератор кода
    /// </summary>
    public required int ManufactureId { get; set; }
}