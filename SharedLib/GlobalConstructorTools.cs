namespace SharedLib;

/// <summary>
/// Глобальные утилиты
/// </summary>
public static partial class GlobalTools
{
    /// <summary>
    /// Получить системное имя элемента
    /// </summary>
    /// <param name="systemNamesManufacture">Коллекция системных имён</param>
    /// <param name="typeDataId">ID:PK элемента (строки) из базы данных</param>
    /// <param name="typeDataName">Имя типа данных</param>
    /// <param name="qualification">Уточнение имени типа данных (если предусмотрены разные реализации)</param>
    /// <returns>Системное имя объекта</returns>
    /// <remarks>
    /// <paramref name="qualification"/>, например, используется для уточнения типа поля документа. Поля документа базово предоставляются как <see cref="FieldFormBaseLowConstructorModel"/>,
    /// но реально существуют в одном из двух производных типов: <see cref="FieldFormConstructorModelDB"/> (простой тип поля: строка, число и т.п.) или <see cref="FieldFormAkaDirectoryConstructorModelDB"/> (поле типа 'справочник/список/перечисление'):
    /// в таком случае тип поля будет указан базовый <see cref="FieldFormBaseLowConstructorModel"/>, а в квалификаторе уточнение имени производного типа (или null, если не требуется)
    /// </remarks>
    public static string? GetSystemName(this SystemNameEntryModel[] systemNamesManufacture, int typeDataId, string typeDataName, string? qualification = null)
    => systemNamesManufacture
        .FirstOrDefault(x => x.Qualification == qualification && x.TypeDataId == typeDataId && x.TypeDataName == typeDataName)?.SystemName;
}