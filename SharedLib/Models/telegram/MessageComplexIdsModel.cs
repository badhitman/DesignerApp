////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// MessageComplexIdsModel
/// </summary>
public class MessageComplexIdsModel
{
    /// <summary>
    /// Telegram
    /// </summary>
    public required int TelegramId { get; set; }

    /// <summary>
    /// База данных
    /// </summary>
    public required int DatabaseId { get; set; }
}