namespace SharedLib;

/// <summary>
/// TelegramUser
/// </summary>
public class TelegramUsersPaginationModel : PaginationResponseModel
{
    /// <summary>
    /// Entries
    /// </summary>
    public required List<TelegramUserViewModel> TelegramUsers { get; set; }
}