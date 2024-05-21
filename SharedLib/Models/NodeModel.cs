////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Вложенная EntryModel
/// </summary>
public class NodeModel : EntryModel
{
    /// <inheritdoc/>
    public static NodeModel Build(int id, string name) => new() { Id = id, Name = name };

    /// <inheritdoc/>
    public static NodeModel Build(string name, int parent_id) => new() { Name = name, ParentId = parent_id };

    /// <summary>
    /// Идентификатор родителя
    /// </summary>
    public int ParentId { get; set; }
}