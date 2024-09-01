using SharedLib;
using MudBlazor;
using System.Security.Claims;

namespace BlazorLib;

/// <summary>
/// Extensions
/// </summary>
public static class Extensions
{
    /// <summary>
    /// Получить данные по текущему пользователю
    /// </summary>
    public static UserInfoMainModel? ReadCurrentUserInfo(this ClaimsPrincipal principal)
    {
        string? userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId is null)
            return null;

        string? givenName = principal.FindFirst(ClaimTypes.GivenName)?.Value;
        string? surName = principal.FindFirst(ClaimTypes.Surname)?.Value;
        string? userName = principal.FindFirst(ClaimTypes.Name)?.Value;
        string? email = principal.FindFirst(ClaimTypes.Email)?.Value;
        string[] roles = principal.FindAll(ClaimTypes.Role).Select(x => x.Value).ToArray();

        long? telegram_id = null;
        string? telegramIdAsString = principal.FindFirst(GlobalStaticConstants.TelegramIdClaimName)?.Value;
        if (!string.IsNullOrWhiteSpace(telegramIdAsString) && long.TryParse(telegramIdAsString, out long tgId))
            telegram_id = tgId;

        return new()
        {
            UserId = userId,
            Email = email,
            TelegramId = telegram_id,
            Surname = surName,
            UserName = userName,
            Roles = [..roles],
            GivenName = givenName,
            Claims = [.. principal.Claims.Where(x => x.Type != ClaimTypes.Role).Select(x => new EntryAltModel() { Id = x.Type, Name = x.Value })],
        };
    }

    /// <inheritdoc/>
    public static void ShowMessagesResponse(this ISnackbar SnackbarRepo, IEnumerable<ResultMessage> messages)
    {
        if (!messages.Any())
            return;

        Severity _style;
        foreach (ResultMessage m in messages)
        {
            _style = m.TypeMessage switch
            {
                ResultTypesEnum.Success => Severity.Success,
                ResultTypesEnum.Info => Severity.Info,
                ResultTypesEnum.Warning => Severity.Warning,
                ResultTypesEnum.Error => Severity.Error,
                _ => Severity.Normal
            };
            SnackbarRepo.Add(m.Text, _style, opt => opt.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
        }
    }

    /// <inheritdoc/>
    public static void Error(this ISnackbar SnackbarRepo, string message)
        => SnackbarRepo.Add(message, Severity.Error, opt => opt.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);

    /// <inheritdoc/>
    public static VerticalDirectionsEnum GetVerticalDirection(this SortDirection sort_direction)
    {
        return sort_direction switch
        {
            SortDirection.Descending => VerticalDirectionsEnum.Down,
            SortDirection.Ascending => VerticalDirectionsEnum.Up,
            _ => VerticalDirectionsEnum.Up
        };
    }
}