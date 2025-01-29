////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Newtonsoft.Json;
using SharedLib;
using System.Net.Http.Json;

namespace ToolsMauiApp;

/// <summary>
/// LogsService
/// </summary>
public class LogsService(IHttpClientFactory HttpClientFactory) : ILogsService
{
    /// <inheritdoc/>
    public async Task<TPaginationResponseModel<NLogRecordModelDB>> LogsSelect(TPaginationRequestModel<LogsSelectRequestModel> req)
    {        
        using HttpClient client = HttpClientFactory.CreateClient(HttpClientsNamesEnum.Tools.ToString());
        using HttpResponseMessage response = await client.PostAsJsonAsync($"/{GlobalStaticConstants.Routes.API_CONTROLLER_NAME}/{GlobalStaticConstants.Routes.TOOLS_CONTROLLER_NAME}/{GlobalStaticConstants.Routes.LOGS_ACTION_NAME}-{GlobalStaticConstants.Routes.SELECT_ACTION_NAME}", req);
        string rj = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<TPaginationResponseModel<NLogRecordModelDB>>(rj)!;
    }
}