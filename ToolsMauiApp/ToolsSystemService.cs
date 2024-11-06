////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Newtonsoft.Json;
using SharedLib;
using System.Net.Http.Json;

namespace ToolsMauiApp;

/// <summary>
/// ToolsSystemService
/// </summary>
public class ToolsSystemExtService(IHttpClientFactory HttpClientFactory) : IToolsSystemExtService
{
    /// <inheritdoc/>
    public async Task<TResponseModel<ToolsFilesResponseModel[]>> GetDirectory(ToolsFilesRequestModel req)
    {
        using HttpClient client = HttpClientFactory.CreateClient(HttpClientsNamesEnum.Tools.ToString());
        using HttpResponseMessage response = await client.PostAsJsonAsync($"/{GlobalStaticConstants.Routes.API_CONTROLLER_NAME}/{GlobalStaticConstants.Routes.TOOLS_CONTROLLER_NAME}/{GlobalStaticConstants.Routes.DIRECTORY_CONTROLLER_NAME}/{GlobalStaticConstants.Routes.GET_ACTION_NAME}", req);
        string rj = await response.Content.ReadAsStringAsync();
        TResponseModel<ToolsFilesResponseModel[]> res = JsonConvert.DeserializeObject<TResponseModel<ToolsFilesResponseModel[]>>(rj)!;

        if (res.Response is null)
            res.AddError("Ошибка обработки запроса");
        else if (res.Response.Length == 0)
            res.AddInfo($"Файлов в удалённой папке нет");
        else
            res.AddInfo($"Файлов в удалённой папке: {res.Response.Length} ({GlobalTools.SizeDataAsString(res.Response.Sum(x => x.Size))})");

        return res;
    }

    /// <inheritdoc/>
    public async Task<TResponseModel<ExpressProfileResponseModel>> GetMe()
    {
        using HttpClient client = HttpClientFactory.CreateClient(HttpClientsNamesEnum.Tools.ToString());
        TResponseModel<ExpressProfileResponseModel> res = await client.GetStringAsync<ExpressProfileResponseModel>($"/{GlobalStaticConstants.Routes.API_CONTROLLER_NAME}/{GlobalStaticConstants.Routes.INFO_CONTROLLER_NAME}/{GlobalStaticConstants.Routes.MY_CONTROLLER_NAME}");

        if (string.IsNullOrWhiteSpace(res.Response?.UserName))
            res.AddError("Пользователь не настроен");
        else if (res.Response.Roles is null || !res.Response.Roles.Any())
            res.AddWarning("Не установлены роли доступа");
        else
            res.AddSuccess("Токен доступа валидный");

        return res;
    }
}