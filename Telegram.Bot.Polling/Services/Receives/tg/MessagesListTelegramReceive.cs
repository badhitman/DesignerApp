////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;
using RemoteCallLib;
using SharedLib;
using DbcLib;

namespace Transmission.Receives.telegram;

/// <summary>
/// Получить сообщения из чата
/// </summary>
public class MessagesListTelegramReceive(IDbContextFactory<TelegramBotContext> tgDbFactory)
    : IResponseReceive<TPaginationRequestModel<int>?, TPaginationResponseModel<MessageTelegramModelDB>?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.MessagesChatsSelectTelegramReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<TPaginationResponseModel<MessageTelegramModelDB>?>> ResponseHandleAction(TPaginationRequestModel<int>? req)
    {
        ArgumentNullException.ThrowIfNull(req);
        if (req.PageSize < 5)
            req.PageSize = 5;

        TResponseModel<TPaginationResponseModel<MessageTelegramModelDB>?> res = new();
        TelegramBotContext context = await tgDbFactory.CreateDbContextAsync();
        IQueryable<MessageTelegramModelDB> q = context
            .Messages
            .Include(x => x.Audio)
            .Include(x => x.Document)
            .Include(x => x.Photo)
            .Include(x => x.Voice)
            .Include(x => x.Video)
            .Where(x => x.ChatId == req.Payload);

        res.Response = new()
        {
            PageNum = req.PageNum,
            PageSize = req.PageSize,
            SortBy = req.SortBy,
            SortingDirection = req.SortingDirection,
            TotalRowsCount = await q.CountAsync(),
            Response = req.SortingDirection == VerticalDirectionsEnum.Up
                  ? await q.OrderBy(x => x.CreatedAtUtc).Skip(req.PageNum * req.PageSize).Take(req.PageSize).Include(x => x.From).ToListAsync()
                  : await q.OrderByDescending(x => x.CreatedAtUtc).Skip(req.PageNum * req.PageSize).Take(req.PageSize).Include(x => x.From).ToListAsync()
        };

        return res;
    }
}