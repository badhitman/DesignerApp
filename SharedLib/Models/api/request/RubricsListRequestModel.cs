////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// RubricsListRequestModel
/// </summary>
public class RubricsListRequestModel : TProjectedRequestModel<int>
{
    /// <summary>
    /// Имя контекста для разделения различных селекторов независимо друг от друга
    /// </summary>
    /// <remarks>
    /// Рубрики Helpdesk имеют значение контекста NULL. А подсистема адресов (Регионы/Города) используют этот эту же службу с указанием на имя контекста: <see cref="GlobalStaticConstants.Routes.ADDRESS_CONTROLLER_NAME"/> 
    /// </remarks>
    public string? ContextName { get; set; }
}