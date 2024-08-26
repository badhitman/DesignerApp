////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;

namespace SharedLib;

/// <summary>
/// TelegramUserBaseModel
/// </summary>
[Index(nameof(TelegramId), IsUnique = true)]
public class TelegramUserBaseModel : TelegramUserLiteModel
{
    /// <summary>
    /// User id (if Identity)
    /// </summary>
    public required string UserIdentityId { get; set; }

    /// <summary>
    /// Уникальный идентификатор Telegram пользователя (или бота)
    /// </summary>
    public long TelegramId { get; set; }

    /// <summary>
    /// Основное сообщение в чате в котором Bot ведёт диалог с пользователем.
    /// Бот может отвечать новым сообщением или редактировать своё ранее отправленное в зависимости от ситуации.
    /// </summary>
    public int? MainTelegramMessageId { get; set; }

    /// <summary>
    /// Тип диалога (имя реализации). Обработчик ответа на входящее сообщение Telegram
    /// </summary>
    public string? DialogTelegramTypeHandler { get; set; }

    /// <inheritdoc/>
    public override string ToString()
    {
        string res = FirstName;
        if (!string.IsNullOrWhiteSpace(LastName))
            res += $" {LastName}";
        if (!string.IsNullOrWhiteSpace(Username))
            res += $" @{Username}";
        return res;
    }

    /// <inheritdoc/>
    public static TelegramUserBaseModel? Build(TelegramUserModelDb? user)
        => user is null
        ? null
        : new()
        {
            FirstName = user.FirstName,
            LastName = user.LastName,
            TelegramId = user.TelegramId,
            IsBot = user.IsBot,
            Username = user.Username,
            Id = user.Id,
            IsDisabled = user.IsDisabled,
            DialogTelegramTypeHandler = user.DialogTelegramTypeHandler,
            MainTelegramMessageId = user.MainTelegramMessageId,
            UserIdentityId = user.UserIdentityId,
        };
}