////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Комплексный/полный ключ доступа к данным мемкеша
/// </summary>
public class MemCasheComplexKeyModel
{
    /// <summary>
    /// Конечный идентификатор/имя ключа доступа к данным мемкеша
    /// </summary>
    public string Id { get; }

    /// <summary>
    /// Префикс ключа доступа к данным мемкеша
    /// </summary>
    public MemCashePrefixModel Pref { get; }

    /// <summary>
    /// Конструктор
    /// </summary>
    /// <param name="set_id">Конечный идентификатор/имя ключа доступа к данным мемкеша</param>
    /// <param name="set_pref">Префикс ключа доступа к данным мемкеша</param>
    public MemCasheComplexKeyModel(string set_id, MemCashePrefixModel set_pref)
    {
        Id = set_id;
        Pref = set_pref;
    }

    /// <summary>
    /// Преобразовать комплексный/полный ключ доступа к данным мемкешуа в строку
    /// </summary>
    /// <returns>Строковое представление комплексного/полного ключа доступа к данным мемкеша</returns>
    public override string ToString()
    {
        string itemId = Id == string.Empty ? "*" : Id;
        if (string.IsNullOrWhiteSpace(Pref.Dictionary))
            return string.Format("{0}:{1}", Pref.Namespace, itemId);
        else
            return string.Format("{0}:{1}", Pref.ToString(), itemId);
    }
}