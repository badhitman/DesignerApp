////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Префикс пути к ключам данным
/// </summary>
/// <remarks>
/// Конструктор
/// </remarks>
/// <param name="_namespace">Пространство имён</param>
/// <param name="_dict">Словарь</param>
public class MemCachePrefixModel(string _namespace, string _dict)
{
    /// <summary>
    /// Пространство имён ключей доступа к данным
    /// </summary>
    public string Namespace { get; } = _namespace;

    /// <summary>
    /// Словарь имён ключей доступа к данным
    /// </summary>
    public string Dictionary { get; } = _dict;

    /// <summary>
    /// Преобразовать объект префикса в строку
    /// </summary>
    /// <returns>Строковое представление объекта префикса</returns>
    public override string ToString() => $"{Namespace}:{Dictionary}";
}