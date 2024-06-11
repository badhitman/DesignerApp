////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Простейший вещественный тип вложенной/древовидной структуры
/// </summary>
public class EntryNestedModel : EntryModel
{
    /// <summary>
    /// Вложенные (дочерние) объекты
    /// </summary>
    public IEnumerable<EntryModel> Childs { get; set; } = [];
}