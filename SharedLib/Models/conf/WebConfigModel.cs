////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// web config
/// </summary>
public class WebConfigModel
{
    /// <summary>
    /// BaseUri
    /// </summary>
    public string? BaseUri { get; set; }

    /// <summary>
    /// BaseUri
    /// </summary>
    public string? ClearBaseUri => string.IsNullOrWhiteSpace(BaseUri) ? string.Empty : BaseUri.EndsWith('/') ? BaseUri[..(BaseUri.Length - 1)] : BaseUri;

    /// <summary>
    /// Допустимая длительность бездействия (в минутах) процедуры привязки Telegram аккаунта к учётной записи сайта
    /// </summary>
    public uint TelegramJoinAccountTokenLifetimeMinutes { get; set; } = 60;

    /// <inheritdoc/>
    public ResponseBaseModel Update(WebConfigModel sender)
    {
        ResponseBaseModel res = new();
        if (BaseUri?.Equals(sender.BaseUri) == true)
            res.AddInfo($"Установка {nameof(BaseUri)} не требуется: значения идентичны");
        else
        {
            BaseUri = sender.BaseUri;
            res.AddSuccess($"Успешно установлен {nameof(BaseUri)}: {sender.BaseUri}");
        }

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