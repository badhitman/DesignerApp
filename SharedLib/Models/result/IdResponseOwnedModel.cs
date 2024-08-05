////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Базовый ответ на запрос с поддержкой идентификатора ведущего объекта вместе с владельцем
/// </summary>
public class IdResponseOwnedModel : TResponseModel<int?>
{
    /// <summary>
    /// Владелец текущего объекта
    /// </summary>
    public EntryModel? CurrentOwnerObject { get; set; }
}