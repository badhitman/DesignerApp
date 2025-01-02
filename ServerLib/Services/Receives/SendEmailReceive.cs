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
    : IResponseReceive<SendEmailRequestModel, ResponseBaseModel>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.SendEmailReceive;

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> ResponseHandleAction(SendEmailRequestModel? email_send)
    {
        ArgumentNullException.ThrowIfNull(email_send);
        _logger.LogInformation($"call `{GetType().Name}`: {JsonConvert.SerializeObject(email_send, GlobalStaticConstants.JsonSerializerSettings)}");
        return await mailRepo.SendEmailAsync(email_send.Email, email_send.Subject, email_send.TextMessage, email_send.MimeType);
    }
}