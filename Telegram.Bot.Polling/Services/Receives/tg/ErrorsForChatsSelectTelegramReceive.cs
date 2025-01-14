////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;
using RemoteCallLib;
using SharedLib;
using DbcLib;

namespace Transmission.Receives.telegram;

/// <summary>
/// Получить ошибки отправок сообщений (для чатов)
/// </summary>
public class ErrorsForChatsSelectTelegramReceive(IDbContextFactory<TelegramBotContext> tgDbFactory)
    : IResponseReceive<TPaginationRequestModel<long[]?>?, TPaginationResponseModel<ErrorSendingMessageTelegramBotModelDB>?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.ErrorsForChatsSelectTelegramReceive;

    /// <inheritdoc/>
    public async Task<TPaginationResponseModel<ErrorSendingMessageTelegramBotModelDB>?> ResponseHandleAction(TPaginationRequestModel<long[]?>? req)
    {
        ArgumentNullException.ThrowIfNull(req);
        if (req.PageSize < 5)
            req.PageSize = 5;

        using TelegramBotContext context = await tgDbFactory.CreateDbContextAsync();

        IQueryable<ErrorSendingMessageTelegramBotModelDB> q = context
            .ErrorsSendingTextMessageTelegramBot
            .AsQueryable();

        if (req.Payload is not null && req.Payload.Length != 0)
            q = q.Where(x => req.Payload.Any(y => y == x.ChatId));

        IQueryable<ErrorSendingMessageTelegramBotModelDB> TakePart(IQueryable<ErrorSendingMessageTelegramBotModelDB> q, VerticalDirectionsEnum direct)
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
            Response = await TakePart(q, req.SortingDirection).ToListAsync(),
        };
    }
}