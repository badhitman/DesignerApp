////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Сообщение из обращения помечается как ответ
/// </summary>
public class VoteIssueRequestModel
{
    /// <summary>
    /// MessageId
    /// </summary>
    public required int MessageId { get; set; }

    /// <summary>
    /// Признак: установить или удалить
    /// </summary>
    public required bool SetStatus { get; set; }
}