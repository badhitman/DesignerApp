////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// TelegramDialog
/// </summary>
public interface ITelegramDialogService
{
    /// <summary>
    /// Обработка входящих сообщений из Telegram
    /// </summary>
    public Task<TelegramDialogResponseModel> TelegramDialogHandle(TelegramDialogRequestModel tgDialog);
}