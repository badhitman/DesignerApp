////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Строки данных таблиц
/// </summary>
public class FieldSessionDocumentDataBaseModel
{
    /// <summary>
    /// Сессия опроса/анкеты
    /// </summary>
    public int SessionId { get; set; }

    /// <summary>
    /// Связь формы со страницей опроса
    /// </summary>
    public int JoinFormId { get; set; }
}