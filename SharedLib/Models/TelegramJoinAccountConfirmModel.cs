namespace SharedLib;

/// <inheritdoc/>
public class TelegramJoinAccountConfirmModel
{
    /// <inheritdoc/>
    public required string Token { get; set; }

    /// <summary>
    /// TelegramUser
    /// </summary>
    public required CheckTelegramUserModel TelegramUser { get; set; }
}