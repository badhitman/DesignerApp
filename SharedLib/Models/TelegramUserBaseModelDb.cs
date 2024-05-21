////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;

namespace SharedLib;

/// <summary>
/// TelegramUserBaseModelDb
/// </summary>
[Index(nameof(TelegramId), IsUnique = true)]
[Index(nameof(Name))]
public class TelegramUserBaseModelDb : EntryCreatedModel
{
    /// <summary>
    /// Уникальный идентификатор Telegram пользователя (или бота)
    /// </summary>
    public long TelegramId { get; set; }

    /// <summary>
    ///  true, if this user is a bot
    /// </summary>
    public bool IsBot { get; set; }

    /// <summary>
    /// User's or bot’s first name
    /// </summary>
    public required string FirstName { get; set; }

    /// <summary>
    /// Optional. User's or bot’s last name
    /// </summary>
    public string? LastName { get; set; }

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
        if (!string.IsNullOrWhiteSpace(Name))
            res += $" @{Name}";
        return res;
    }

    /// <inheritdoc/>
    public static TelegramUserBaseModelDb? Build(TelegramUserModelDb? user)
        => user is null
        ? null
        : new()
        {
            FirstName = user.FirstName,
            LastName = user.LastName,
            TelegramId = user.TelegramId,
            IsBot = user.IsBot,
            Name = user.Name,
            CreatedAt = user.CreatedAt,
            Id = user.Id,
            IsDeleted = user.IsDeleted,
            DialogTelegramTypeHandler = user.DialogTelegramTypeHandler,
            MainTelegramMessageId = user.MainTelegramMessageId,
        };

    /// <inheritdoc/>
    public static TelegramUserBaseModelDb Build(CheckTelegramUserHandleModel user)
        => new()
        {
            FirstName = user.FirstName,
            LastName = user.LastName,
            TelegramId = user.TelegramUserId,
            IsBot = user.IsBot,
            Name = user.Username,
        };
}