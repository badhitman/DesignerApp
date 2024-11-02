////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Комплексный/полный ключ доступа к данным Cache
/// </summary>
/// <remarks>
/// Конструктор
/// </remarks>
/// <param name="set_id">Конечный идентификатор/имя ключа доступа к данным Cache</param>
/// <param name="set_pref">Префикс ключа доступа к данным Cache</param>
public class MemCacheComplexKeyModel(string set_id, MemCachePrefixModel set_pref)
{
    /// <summary>
    /// Конечный идентификатор/имя ключа доступа к данным Cache
    /// </summary>
    public string Id { get; } = set_id;

    /// <summary>
    /// Префикс ключа доступа к данным Cache
    /// </summary>
    public MemCachePrefixModel Pref { get; } = set_pref;

    /// <summary>
    /// Преобразовать комплексный/полный ключ доступа к данным Cache в строку
    /// </summary>
    /// <returns>Строковое представление комплексного/полного ключа доступа к данным Cache</returns>
    public override string ToString()
    {
        string itemId = Id == string.Empty ? "*" : Id;
        if (string.IsNullOrWhiteSpace(Pref.Dictionary))
            return string.Format("{0}:{1}", Pref.Namespace, itemId);
        else
            return string.Format("{0}:{1}", Pref.ToString(), itemId);
    }
}