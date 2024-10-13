////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// SelectArticlesRequestModel
/// </summary>
public class SelectArticlesRequestModel : SelectRequestBaseModel
{
    /// <summary>
    /// Загрузить данные по тэгам
    /// </summary>
    public bool IncludeTags { get; set; }
}