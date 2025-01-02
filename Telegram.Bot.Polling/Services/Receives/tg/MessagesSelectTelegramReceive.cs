////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore;
using RemoteCallLib;
using SharedLib;
using DbcLib;

namespace Transmission.Receives.telegram;

/// <summary>
/// Получить сообщения из чата
/// </summary>
public class MessagesSelectTelegramReceive(IDbContextFactory<TelegramBotContext> tgDbFactory)
    : IResponseReceive<TPaginationRequestModel<SearchMessagesChatModel>, TPaginationResponseModel<MessageTelegramModelDB>>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.MessagesChatsSelectTelegramReceive;

    /// <inheritdoc/>
    public async Task<TPaginationResponseModel<MessageTelegramModelDB>?> ResponseHandleAction(TPaginationRequestModel<SearchMessagesChatModel>? req)
    {
        ArgumentNullException.ThrowIfNull(req);
        if (req.PageSize < 5)
            req.PageSize = 5;

        using TelegramBotContext context = await tgDbFactory.CreateDbContextAsync();
        IQueryable<MessageTelegramModelDB> q = context
            .Messages
            .Where(x => x.ChatId == req.Payload.ChatId);

        if (!string.IsNullOrWhiteSpace(req.Payload.SearchQuery))
        {
            req.Payload.SearchQuery = req.Payload.SearchQuery.ToUpper();
            q = q.Where(x => (x.NormalizedTextUpper != null && x.NormalizedTextUpper.Contains(req.Payload.SearchQuery)) || (x.NormalizedCaptionUpper != null && x.NormalizedCaptionUpper.Contains(req.Payload.SearchQuery)));
        }

        IIncludableQueryable<MessageTelegramModelDB, UserTelegramModelDB?> Include(IQueryable<MessageTelegramModelDB> query)
        {
            return query.Include(x => x.Audio)
            .Include(x => x.Document)
            .Include(x => x.Photo)
            .Include(x => x.Voice)
            .Include(x => x.Video)
            .Include(x => x.From)
            ;
        }

        IQueryable<MessageTelegramModelDB> TakePart(IQueryable<MessageTelegramModelDB> q, VerticalDirectionsEnum direct)
        {
            return direct == VerticalDirectionsEnum.Up
                ? q.OrderBy(x => x.CreatedAtUtc).Skip(req.PageNum * req.PageSize).Take(req.PageSize)
                : q.OrderByDescending(x => x.CreatedAtUtc).Skip(req.PageNum * req.PageSize).Take(req.PageSize);
        }

        return new()
        {
            PageNum = req.PageNum,
            PageSize = req.PageSize,
            SortBy = req.SortBy,
            SortingDirection = req.SortingDirection,
            TotalRowsCount = await q.CountAsync(),
            Response = await Include(TakePart(q, req.SortingDirection)).ToListAsync()
        };
    }
}