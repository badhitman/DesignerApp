////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Newtonsoft.Json;
using SharedLib;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;

namespace ToolsMauiApp;

/// <summary>
/// ToolsSystemService
/// </summary>
public class ToolsSystemExtService(IHttpClientFactory HttpClientFactory) : IToolsSystemExtService
{
    static string snh = nameof(ConfigStoreModel.RemoteDirectory);

    /// <inheritdoc/>
    public async Task<TResponseModel<bool>> DeleteFile(DeleteRemoteFileRequestModel req)
    {
        using HttpClient client = HttpClientFactory.CreateClient(HttpClientsNamesEnum.Tools.ToString());
        using HttpResponseMessage response = await client.PostAsJsonAsync($"/{GlobalStaticConstants.Routes.API_CONTROLLER_NAME}/{GlobalStaticConstants.Routes.TOOLS_CONTROLLER_NAME}/{GlobalStaticConstants.Routes.FILE_CONTROLLER_NAME}-{GlobalStaticConstants.Routes.DELETE_ACTION_NAME}", req);
        string rj = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<TResponseModel<bool>>(rj)!;
    }

    /// <inheritdoc/>
    public async Task<TResponseModel<List<ToolsFilesResponseModel>>> GetDirectory(ToolsFilesRequestModel req)
    {
        using HttpClient client = HttpClientFactory.CreateClient(HttpClientsNamesEnum.Tools.ToString());
        using HttpResponseMessage response = await client.PostAsJsonAsync($"/{GlobalStaticConstants.Routes.API_CONTROLLER_NAME}/{GlobalStaticConstants.Routes.TOOLS_CONTROLLER_NAME}/{GlobalStaticConstants.Routes.DIRECTORY_CONTROLLER_NAME}/{GlobalStaticConstants.Routes.GET_ACTION_NAME}", req);
        string rj = await response.Content.ReadAsStringAsync();
        TResponseModel<List<ToolsFilesResponseModel>> res = JsonConvert.DeserializeObject<TResponseModel<List<ToolsFilesResponseModel>>>(rj)!;

        if (res.Response is null)
            res.AddError("Ошибка обработки запроса");
        else if (res.Response.Count == 0)
            res.AddInfo($"Файлов в удалённой папке нет");
        else
        {
            res.AddInfo($"Файлов в удалённой папке: {res.Response.Count} ({GlobalTools.SizeDataAsString(res.Response.Sum(x => x.Size))})");
            res.Response.ForEach(x =>
            {
                x.FullName = x.FullName.Replace('/', Path.DirectorySeparatorChar).Replace('\\', Path.DirectorySeparatorChar);
                x.ScopeName = x.ScopeName.Replace('/', Path.DirectorySeparatorChar).Replace('\\', Path.DirectorySeparatorChar);
            });
        }
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

    /// <inheritdoc/>
    public async Task<TResponseModel<bool>> UpdateFile(ToolsFilesResponseModel tFile, byte[] file_bytes)
    {
        TResponseModel<bool> res = new();
        if (string.IsNullOrWhiteSpace(MauiProgram.ConfigStore.Response?.RemoteDirectory))
        {
            res.AddError("Настройки не инициализированы");
            return res;
        }

        using HttpClient httpClient = HttpClientFactory.CreateClient(HttpClientsNamesEnum.Tools.ToString());

        MultipartFormDataContent form = new()
        {
            //{ new StringContent(nameof(tFile.SafeScopeName)), tFile.SafeScopeName },
            { new ByteArrayContent(file_bytes, 0, file_bytes.Length), "uploadedFile", tFile.SafeScopeName }
        };
        if (!httpClient.DefaultRequestHeaders.Any(x => x.Key == snh))
            httpClient.DefaultRequestHeaders.Add(snh, Convert.ToBase64String(Encoding.UTF8.GetBytes(MauiProgram.ConfigStore.Response.RemoteDirectory)));
        HttpResponseMessage response = await httpClient.PostAsync($"/{GlobalStaticConstants.Routes.API_CONTROLLER_NAME}/{GlobalStaticConstants.Routes.TOOLS_CONTROLLER_NAME}/{GlobalStaticConstants.Routes.FILE_CONTROLLER_NAME}-{GlobalStaticConstants.Routes.UPDATE_ACTION_NAME}", form);

        response.EnsureSuccessStatusCode();
        httpClient.Dispose();
        string sd = response.Content.ReadAsStringAsync().Result;
        return JsonConvert.DeserializeObject<TResponseModel<bool>>(sd)!;
    }
}