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
    /// Загрузить данные по тэгам и рубрикам
    /// </summary>
    public bool IncludeExternal { get; set; }
}