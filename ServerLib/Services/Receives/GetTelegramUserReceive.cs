﻿////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using Microsoft.Extensions.Logging;
using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.web;

/// <inheritdoc/>
public class GetTelegramUserReceive(ITelegramWebService tgWebRepo, ILogger<GetTelegramUserReceive> _logger)
    : IResponseReceive<long, TelegramUserBaseModelDb?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.GetTelegramUserReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<TelegramUserBaseModelDb?>> ResponseHandleAction(long payload)
    {
        TResponseModel<TelegramUserBaseModelDb?> res = new();
        string msg;
        if (payload == default)
        {
            msg = "remote call [payload] == default: error 76FC1696-0849-42F9-BFEA-57622031EB8F";
            _logger.LogError(msg);
            res.AddError(msg);
            return res;
        }

        try
        {
            res = await tgWebRepo.GetTelegramUserCachedInfo(payload);
        }
        catch (Exception ex)
        {
            msg = $"ошибка обработки запроса `{payload}`. error {{52351B07-1621-4F50-8F6A-CF97E39F3159}}";
            _logger.LogError(ex, msg);
            res.AddError(msg);
            res.Messages.InjectException(ex);
        }

        return res;
    }
}