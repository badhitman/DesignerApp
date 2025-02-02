////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SharedLib;
using System.Net.Http.Json;

namespace ToolsMauiApp;

/// <summary>
/// LogsService
/// </summary>
public class LogsService(IHttpClientFactory HttpClientFactory, ILogger<LogsService> loggerRepo) : ILogsService
{
    /// <inheritdoc/>
    public async Task<TPaginationResponseModel<NLogRecordModelDB>> LogsSelect(TPaginationRequestModel<LogsSelectRequestModel> req)
    {
        using HttpClient client = HttpClientFactory.CreateClient(HttpClientsNamesEnum.Tools.ToString());
        using HttpResponseMessage response = await client.PostAsJsonAsync($"/{GlobalStaticConstants.Routes.API_CONTROLLER_NAME}/{GlobalStaticConstants.Routes.TOOLS_CONTROLLER_NAME}/{GlobalStaticConstants.Routes.LOGS_ACTION_NAME}-{GlobalStaticConstants.Routes.SELECT_ACTION_NAME}", req);
        string rj = await response.Content.ReadAsStringAsync();

        try
        {
            return JsonConvert.DeserializeObject<TPaginationResponseModel<NLogRecordModelDB>>(rj)!;
        }
        catch (Exception ex)
        {
            TPaginationResponseModel<NLogRecordModelDB> res = new();
            loggerRepo.LogError(ex, rj);
            return res;
        }
    }

    /// <inheritdoc/>
    public async Task<TResponseModel<LogsMetadataResponseModel>> MetadataLogs(PeriodDatesTimesModel req)
    {
        using HttpClient client = HttpClientFactory.CreateClient(HttpClientsNamesEnum.Tools.ToString());
        using HttpResponseMessage response = await client.PostAsJsonAsync($"/{GlobalStaticConstants.Routes.API_CONTROLLER_NAME}/{GlobalStaticConstants.Routes.TOOLS_CONTROLLER_NAME}/{GlobalStaticConstants.Routes.LOGS_ACTION_NAME}-{GlobalStaticConstants.Routes.METADATA_CONTROLLER_NAME}", req);
        string rj = await response.Content.ReadAsStringAsync();

        try
        {
            return JsonConvert.DeserializeObject<TResponseModel<LogsMetadataResponseModel>>(rj)!;
        }
        catch (Exception ex)
        {
            TResponseModel<LogsMetadataResponseModel> res = new();
            loggerRepo.LogError(ex, rj);
            res.Messages.InjectException(ex);
            return res;
        }
    }
}