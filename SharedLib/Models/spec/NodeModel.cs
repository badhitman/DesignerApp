////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Вложенная EntryModel
/// </summary>
public class NodeModel : EntryModel
{
    /// <summary>
    /// Идентификатор родителя
    /// </summary>
    public int ParentId { get; set; }

    /// <inheritdoc/>
    public static NodeModel Build(int id, string name) => new() { Id = id, Name = name };

    /// <inheritdoc/>
    public static NodeModel Build(string name, int parent_id) => new() { Name = name, ParentId = parent_id };
}