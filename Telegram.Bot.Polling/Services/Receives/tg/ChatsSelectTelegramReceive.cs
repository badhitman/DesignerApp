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
public class ChatsSelectTelegramReceive(IDbContextFactory<TelegramBotContext> tgDbFactory)
    : IResponseReceive<TPaginationRequestModel<string?>?, TPaginationResponseModel<ChatTelegramModelDB>?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.ChatsSelectTelegramReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<TPaginationResponseModel<ChatTelegramModelDB>?>> ResponseHandleAction(TPaginationRequestModel<string?>? req)
    {
        ArgumentNullException.ThrowIfNull(req);
        if (req.PageSize < 5)
            req.PageSize = 5;

        TResponseModel<TPaginationResponseModel<ChatTelegramModelDB>?> res = new();
        TelegramBotContext context = await tgDbFactory.CreateDbContextAsync();

        IQueryable<ChatTelegramModelDB> q = context
            .Chats
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(req.Payload))
        {
            q = q.Where(x =>
            (x.NormalizedFirstNameUpper != null && x.NormalizedFirstNameUpper.Contains(req.Payload)) ||
            (x.NormalizedLastNameUpper != null && x.NormalizedLastNameUpper.Contains(req.Payload)) ||
            (x.NormalizedTitleUpper != null && x.NormalizedTitleUpper.Contains(req.Payload)) ||
            (x.NormalizedUsernameUpper != null && x.NormalizedUsernameUpper.Contains(req.Payload))
            );
        }

        IQueryable<ChatTelegramModelDB> TakePart(IQueryable<ChatTelegramModelDB> q, VerticalDirectionsEnum direct)
        {
            return direct == VerticalDirectionsEnum.Up
                ? q.OrderBy(x => x.LastUpdateUtc).Skip(req.PageNum * req.PageSize).Take(req.PageSize)
                : q.OrderByDescending(x => x.LastUpdateUtc).Skip(req.PageNum * req.PageSize).Take(req.PageSize);
        }

        res.Response = new()
        {
            PageNum = req.PageNum,
            PageSize = req.PageSize,
            SortBy = req.SortBy,
            SortingDirection = req.SortingDirection,
            TotalRowsCount = await q.CountAsync(),
            Response = await TakePart(q, req.SortingDirection).ToListAsync(),
        };

        return res;
    }
}