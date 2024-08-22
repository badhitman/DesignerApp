////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using System.ComponentModel;

namespace SharedLib;

/// <summary>
/// Типы чатов (Telegram)
/// </summary>
public enum ChatsTypesTelegramEnum
{
    /// <summary>
    /// Normal one to one
    /// </summary>
    [Description("Лично")]
    Private = 1,

    /// <summary>
    /// Normal group chat
    /// </summary>
    [Description("Группа")]
    Group,

    /// <summary>
    /// A channel
    /// </summary>
    [Description("Канал")]
    Channel,

    /// <summary>
    /// A supergroup
    /// </summary>
    [Description("Супергруппа")]
    Supergroup,

    /// <summary>
    /// “sender” for a private chat with the inline query sender
    /// </summary>
    [Description("Inline sender")]
    Sender
}