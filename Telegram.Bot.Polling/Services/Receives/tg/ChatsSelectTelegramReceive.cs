////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;
using RemoteCallLib;
using SharedLib;
using DbcLib;

namespace Transmission.Receives.telegram;

/// <summary>
/// Получить чаты
/// </summary>
public class ChatsSelectTelegramReceive(IDbContextFactory<TelegramBotContext> tgDbFactory) : IResponseReceive<TPaginationRequestModel<string?>?, TPaginationResponseModel<ChatTelegramModelDB>?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.ChatsSelectTelegramReceive;

    /// <inheritdoc/>
    public async Task<TPaginationResponseModel<ChatTelegramModelDB>?> ResponseHandleAction(TPaginationRequestModel<string?>? req)
    {
        ArgumentNullException.ThrowIfNull(req);
        if (req.PageSize < 5)
            req.PageSize = 5;

        using TelegramBotContext context = await tgDbFactory.CreateDbContextAsync();

        IQueryable<ChatTelegramModelDB> q = context
            .Chats
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(req.Payload))
        {
            string find_req = req.Payload.ToUpper();
            q = q.Where(x =>
            (x.NormalizedFirstNameUpper != null && x.NormalizedFirstNameUpper.Contains(find_req)) ||
            (x.NormalizedLastNameUpper != null && x.NormalizedLastNameUpper.Contains(find_req)) ||
            (x.NormalizedTitleUpper != null && x.NormalizedTitleUpper.Contains(find_req)) ||
            (x.NormalizedUsernameUpper != null && x.NormalizedUsernameUpper.Contains(find_req))
            );
        }

        IQueryable<ChatTelegramModelDB> TakePart(IQueryable<ChatTelegramModelDB> q, VerticalDirectionsEnum direct)
        {
            return direct == VerticalDirectionsEnum.Up
                ? q.OrderBy(x => x.LastUpdateUtc).Skip(req.PageNum * req.PageSize).Take(req.PageSize)
                : q.OrderByDescending(x => x.LastUpdateUtc).Skip(req.PageNum * req.PageSize).Take(req.PageSize);
        }

        return new()
        {
            PageNum = req.PageNum,
            PageSize = req.PageSize,
            SortBy = req.SortBy,
            SortingDirection = req.SortingDirection,
            TotalRowsCount = await q.CountAsync(),
            Response = await TakePart(q, req.SortingDirection).ToListAsync(),
        };
    }
}