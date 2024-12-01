////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Newtonsoft.Json;
using RemoteCallLib;
using SharedLib;
using System.Net.Http.Json;

namespace Transmission.Receives.telegram;

/// <summary>
/// Send Wappi message
/// </summary>
public class SendWappiMessageReceive(
    ILogger<SendWappiMessageReceive> _logger,
    IHttpClientFactory HttpClientFactory,
    ISerializeStorageRemoteTransmissionService StorageTransmissionRepo)
    : IResponseReceive<EntryAltExtModel?, SendMessageResponseModel?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.SendWappiMessageReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<SendMessageResponseModel?>> ResponseHandleAction(EntryAltExtModel? req)
    {
        ArgumentNullException.ThrowIfNull(req);
        _logger.LogInformation($"call `{GetType().Name}`: {JsonConvert.SerializeObject(req)}");
        TResponseModel<SendMessageResponseModel?> res = new();

        TResponseModel<string?> wappiToken = await StorageTransmissionRepo.ReadParameter<string?>(GlobalStaticConstants.CloudStorageMetadata.WappiTokenApi);
        TResponseModel<string?> wappiProfileId = await StorageTransmissionRepo.ReadParameter<string?>(GlobalStaticConstants.CloudStorageMetadata.WappiProfileId);

        if (!wappiToken.Success() || string.IsNullOrWhiteSpace(wappiToken.Response) || !wappiProfileId.Success() || string.IsNullOrWhiteSpace(wappiProfileId.Response))
        {
            _logger.LogError($"Не удалось отправить сообщение Wappi ({req}): не удалось прочитать настройки");

            if (wappiToken.Messages.Count != 0)
                res.AddRangeMessages(wappiToken.Messages);

            if (wappiProfileId.Messages.Count != 0)
                res.AddRangeMessages(wappiProfileId.Messages);

            return res;
        }

        using HttpClient client = HttpClientFactory.CreateClient(HttpClientsNamesEnum.Wappi.ToString());
        if (!client.DefaultRequestHeaders.Any(x => x.Key == "Authorization"))
            client.DefaultRequestHeaders.Add("Authorization", wappiToken.Response);

        using HttpResponseMessage response = await client.PostAsJsonAsync($"/api/sync/message/send?profile_id={wappiProfileId.Response}", new SendMessageRequestModel() { Body = req.Text, Recipient = req.Number });

        if (!response.IsSuccessStatusCode)
        {
            string _msg = $"http err (wappi): {response.StatusCode} ({response.Content.ReadAsStringAsync()})\n\n{JsonConvert.SerializeObject(req, GlobalStaticConstants.JsonSerializerSettings)}";
            _logger.LogError(_msg);
            res.AddError(_msg);
        }
        else
        {
            string rj = await response.Content.ReadAsStringAsync();
            res.Response = JsonConvert.DeserializeObject<SendMessageResponseModel>(rj);
        }

        return res;
    }
}