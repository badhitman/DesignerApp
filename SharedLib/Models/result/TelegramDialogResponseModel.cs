////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Ответ на входящее сообщение из Telegram
/// </summary>
public class TelegramDialogResponseModel : TResponseModel<string?>
{
    /// <summary>
    /// Клавиатура ответа.
    /// </summary>
    public List<IEnumerable<ButtonActionModel>>? ReplyKeyboard { get; set; }

    /// <summary>
    /// Основное сообщение в чате в котором Bot ведёт диалог с пользователем.
    /// Бот может отвечать новым сообщением или редактировать своё ранее отправленное в зависимости от ситуации.
    /// Если параметр указан, то будет попытка ответить не новым сообщением, а изменением существующего.
    /// Если редактирование не получится (равно если параметр не указан), то бот отправит это сообщение как новое.
    /// </summary>
    public int? MainTelegramMessageId { get; set; }
}