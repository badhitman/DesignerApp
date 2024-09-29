////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.web;

/// <summary>
/// Отправка Email - receive
/// </summary>
public class SendEmailReceive(IMailProviderService mailRepo, ILogger<SendEmailReceive> _logger)
    : IResponseReceive<SendEmailRequestModel?, bool>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.SendEmailReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<bool>> ResponseHandleAction(SendEmailRequestModel? email_send)
    {
        ArgumentNullException.ThrowIfNull(email_send);
        _logger.LogInformation($"call `{GetType().Name}`: {JsonConvert.SerializeObject(email_send, GlobalStaticConstants.JsonSerializerSettings)}");
        TResponseModel<bool> res = new();
        ResponseBaseModel mail_result = await mailRepo.SendEmailAsync(email_send.Email, email_send.Subject, email_send.TextMessage, email_send.MimeType);
        res.AddRangeMessages(mail_result.Messages);
        res.Response = mail_result.Success();
        return res;
    }
}