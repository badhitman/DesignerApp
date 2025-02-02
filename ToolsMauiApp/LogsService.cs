////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Newtonsoft.Json;
using SharedLib;

namespace ToolsMauiApp;

/// <summary>
/// LogsService
/// </summary>
public class LogsService : ILogsService
{
    JsonSerializerOptions _serializerOptions;

    /// <summary>
    /// LogsService
    /// </summary>
    public LogsService()
    {
        _serializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };
    }

    /// <inheritdoc/>
    public async Task<TPaginationResponseModel<NLogRecordModelDB>> LogsSelect(TPaginationRequestModel<LogsSelectRequestModel> req)
    {
        using HttpClient _client = new();
        _client.DefaultRequestHeaders.Add("token-access", MauiProgram.ConfigStore.Response?.AccessToken);

        string json = System.Text.Json.JsonSerializer.Serialize(req, _serializerOptions);
        StringContent content = new(json, Encoding.UTF8, "application/json");

        using HttpResponseMessage response = await _client.PostAsync(new Uri($"{MauiProgram.ConfigStore.Response?.ApiAddress}/{GlobalStaticConstants.Routes.API_CONTROLLER_NAME}/{GlobalStaticConstants.Routes.TOOLS_CONTROLLER_NAME}/{GlobalStaticConstants.Routes.LOGS_ACTION_NAME}-{GlobalStaticConstants.Routes.SELECT_ACTION_NAME}"), content);

        // HttpResponseMessage response = await _client.PostAsJsonAsync($"{MauiProgram.ConfigStore.Response?.ApiAddress}/{GlobalStaticConstants.Routes.API_CONTROLLER_NAME}/{GlobalStaticConstants.Routes.TOOLS_CONTROLLER_NAME}/{GlobalStaticConstants.Routes.LOGS_ACTION_NAME}-{GlobalStaticConstants.Routes.SELECT_ACTION_NAME}", req);
        string rj = await response.Content.ReadAsStringAsync();

        return JsonConvert.DeserializeObject<TPaginationResponseModel<NLogRecordModelDB>>(rj)!;
    }

    /// <inheritdoc/>
    public async Task<TResponseModel<LogsMetadataResponseModel>> MetadataLogs(PeriodDatesTimesModel req)
    {
        using HttpClient _client = new();
        _client.DefaultRequestHeaders.Add("token-access", MauiProgram.ConfigStore.Response?.AccessToken);

        string json = System.Text.Json.JsonSerializer.Serialize(req, _serializerOptions);
        StringContent content = new(json, Encoding.UTF8, "application/json");

        using HttpResponseMessage response = await _client.PostAsync(new Uri($"{MauiProgram.ConfigStore.Response?.ApiAddress}/{GlobalStaticConstants.Routes.API_CONTROLLER_NAME}/{GlobalStaticConstants.Routes.TOOLS_CONTROLLER_NAME}/{GlobalStaticConstants.Routes.LOGS_ACTION_NAME}-{GlobalStaticConstants.Routes.METADATA_CONTROLLER_NAME}"), content);
        //using HttpResponseMessage response = await _client.PostAsJsonAsync($"/{GlobalStaticConstants.Routes.API_CONTROLLER_NAME}/{GlobalStaticConstants.Routes.TOOLS_CONTROLLER_NAME}/{GlobalStaticConstants.Routes.LOGS_ACTION_NAME}-{GlobalStaticConstants.Routes.METADATA_CONTROLLER_NAME}", req);
        string rj = await response.Content.ReadAsStringAsync();

        try
        {
            return JsonConvert.DeserializeObject<TResponseModel<LogsMetadataResponseModel>>(rj)!;
        }
        catch (Exception ex)
        {
            TResponseModel<LogsMetadataResponseModel> res = new();

            res.Messages.InjectException(ex);
            return res;
        }
    }
}