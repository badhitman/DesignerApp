////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using static SharedLib.GlobalStaticConstants;
using System.Text.Json;
using Newtonsoft.Json;
using System.Text;
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
    public async Task<TPaginationResponseModel<NLogRecordModelDB>> GoToPageForRow(TPaginationRequestModel<int> req)
    {

        using HttpClient _client = new();
        _client.DefaultRequestHeaders.Add("token-access", MauiProgram.ConfigStore.Response?.AccessToken);

        string json = System.Text.Json.JsonSerializer.Serialize(req, _serializerOptions);
        StringContent content = new(json, Encoding.UTF8, "application/json");

        using HttpResponseMessage response = await _client.PostAsync(new Uri($"/{Routes.API_CONTROLLER_NAME}/{Routes.TOOLS_CONTROLLER_NAME}/{Routes.LOGS_ACTION_NAME}-{Routes.PAGE_ACTION_NAME}/{Routes.GOTO_ACTION_NAME}-for-{Routes.RECORD_CONTROLLER_NAME}"), content);

        string rj = await response.Content.ReadAsStringAsync();

        return JsonConvert.DeserializeObject<TPaginationResponseModel<NLogRecordModelDB>>(rj)!;
    }

    /// <inheritdoc/>
    public async Task<TPaginationResponseModel<NLogRecordModelDB>> LogsSelect(TPaginationRequestModel<LogsSelectRequestModel> req)
    {
        using HttpClient _client = new();
        _client.DefaultRequestHeaders.Add("token-access", MauiProgram.ConfigStore.Response?.AccessToken);

        string json = System.Text.Json.JsonSerializer.Serialize(req, _serializerOptions);
        StringContent content = new(json, Encoding.UTF8, "application/json");

        using HttpResponseMessage response = await _client.PostAsync(new Uri($"{MauiProgram.ConfigStore.Response?.ApiAddress}/{Routes.API_CONTROLLER_NAME}/{Routes.TOOLS_CONTROLLER_NAME}/{Routes.LOGS_ACTION_NAME}-{Routes.SELECT_ACTION_NAME}"), content);
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