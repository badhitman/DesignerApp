////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////


namespace SharedLib;

/// <summary>
/// TelegramBot config
/// </summary>
public class TelegramBotConfigModel : WebConfigModel
{
    /// <summary>
    /// Допустимая длительность бездействия (в минутах) процедуры привязки Telegram аккаунта к учётной записи сайта
    /// </summary>
    public uint TelegramJoinAccountTokenLifetimeMinutes { get; set; } = 60;

    /// <inheritdoc/>
    public ResponseBaseModel Update(TelegramBotConfigModel sender)
    {
        ResponseBaseModel res = Uri.TryCreate(sender.BaseUri, uriKind: UriKind.Absolute, out _)
            ? Update(sender.BaseUri)
            : new();

        if (TelegramJoinAccountTokenLifetimeMinutes == sender.TelegramJoinAccountTokenLifetimeMinutes)
            res.AddInfo($"Установка {nameof(TelegramJoinAccountTokenLifetimeMinutes)} не требуется: значения идентичны");
        else
        {
            TelegramJoinAccountTokenLifetimeMinutes = sender.TelegramJoinAccountTokenLifetimeMinutes;
            res.AddSuccess($"Успешно установлен {nameof(TelegramJoinAccountTokenLifetimeMinutes)}: {sender.TelegramJoinAccountTokenLifetimeMinutes}");
        }
        return res;
    }
}