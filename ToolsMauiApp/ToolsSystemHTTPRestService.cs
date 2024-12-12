////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using System.Net.Http.Json;
using Newtonsoft.Json;
using System.Text;
using SharedLib;

namespace ToolsMauiApp;

/// <summary>
/// ToolsSystemHTTPRestService
/// </summary>
public class ToolsSystemHTTPRestService(IHttpClientFactory HttpClientFactory) : IToolsSystemHTTPRestService
{
    //private static readonly string snh = nameof(ConfigStoreModel.RemoteDirectory);


    /// <inheritdoc/>
    public async Task<ResponseBaseModel> PartUpload(SessionFileRequestModel req)
    {
        using HttpClient httpClient = HttpClientFactory.CreateClient(HttpClientsNamesEnum.Tools.ToString());

        MultipartFormDataContent form = new()
        {
            { new ByteArrayContent(req.Data, 0, req.Data.Length), "uploadedFile", Path.GetFileName(req.FileName) }
        };

        string routeUri = $"/{GlobalStaticConstants.Routes.API_CONTROLLER_NAME}/{GlobalStaticConstants.Routes.TOOLS_CONTROLLER_NAME}/{GlobalStaticConstants.Routes.PART_CONTROLLER_NAME}-{GlobalStaticConstants.Routes.UPLOAD_ACTION_NAME}";

        routeUri += $"?{GlobalStaticConstants.Routes.SESSION_CONTROLLER_NAME}_{GlobalStaticConstants.Routes.TOKEN_CONTROLLER_NAME}={Convert.ToBase64String(Encoding.UTF8.GetBytes(req.SessionId))}";
        routeUri += $"&{GlobalStaticConstants.Routes.FILE_CONTROLLER_NAME}_{GlobalStaticConstants.Routes.TOKEN_CONTROLLER_NAME}={Convert.ToBase64String(Encoding.UTF8.GetBytes(req.FileId))}";

        HttpResponseMessage response = await httpClient.PostAsync(routeUri, form);

        response.EnsureSuccessStatusCode();
        httpClient.Dispose();
        string rj = response.Content.ReadAsStringAsync().Result;

        return JsonConvert.DeserializeObject<ResponseBaseModel>(rj)!;
    }

    /// <inheritdoc/>
    public async Task<TResponseModel<PartUploadSessionModel>> PartUploadSessionStart(PartUploadSessionStartRequestModel req)
    {
        using HttpClient client = HttpClientFactory.CreateClient(HttpClientsNamesEnum.Tools.ToString());
        using HttpResponseMessage response = await client.PostAsJsonAsync($"/{GlobalStaticConstants.Routes.API_CONTROLLER_NAME}/{GlobalStaticConstants.Routes.TOOLS_CONTROLLER_NAME}/{GlobalStaticConstants.Routes.SESSION_CONTROLLER_NAME}-{GlobalStaticConstants.Routes.PART_CONTROLLER_NAME}-{GlobalStaticConstants.Routes.UPLOAD_ACTION_NAME}-{GlobalStaticConstants.Routes.START_ACTION_NAME}", req);
        string rj = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<TResponseModel<PartUploadSessionModel>>(rj)!;
    }

    /// <inheritdoc/>
    public async Task<TResponseModel<bool>> DeleteFile(DeleteRemoteFileRequestModel req)
    {
        using HttpClient client = HttpClientFactory.CreateClient(HttpClientsNamesEnum.Tools.ToString());
        using HttpResponseMessage response = await client.PostAsJsonAsync($"/{GlobalStaticConstants.Routes.API_CONTROLLER_NAME}/{GlobalStaticConstants.Routes.TOOLS_CONTROLLER_NAME}/{GlobalStaticConstants.Routes.FILE_CONTROLLER_NAME}-{GlobalStaticConstants.Routes.DELETE_ACTION_NAME}", req);
        string rj = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<TResponseModel<bool>>(rj)!;
    }

    /// <inheritdoc/>
    public async Task<TResponseModel<string>> ExeCommand(ExeCommandModel req)
    {
        using HttpClient client = HttpClientFactory.CreateClient(HttpClientsNamesEnum.Tools.ToString());
        using HttpResponseMessage response = await client.PostAsJsonAsync($"/{GlobalStaticConstants.Routes.API_CONTROLLER_NAME}/{GlobalStaticConstants.Routes.TOOLS_CONTROLLER_NAME}/{GlobalStaticConstants.Routes.CMD_CONTROLLER_NAME}/{GlobalStaticConstants.Routes.EXE_ACTION_NAME}", req);
        string rj = await response.Content.ReadAsStringAsync();

        return JsonConvert.DeserializeObject<TResponseModel<string>>(rj)!;
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
    public async Task<TResponseModel<string>> UpdateFile(string fileScopeName, string remoteDirectory, byte[] bytes)
    {
        TResponseModel<string> res = new();
        if (string.IsNullOrWhiteSpace(remoteDirectory))
        {
            res.AddError("Настройки не инициализированы");
            return res;
        }

        using HttpClient httpClient = HttpClientFactory.CreateClient(HttpClientsNamesEnum.Tools.ToString());

        MultipartFormDataContent form = new()
        {
            { new ByteArrayContent(bytes, 0, bytes.Length), "uploadedFile", fileScopeName }
        };

        string routeUri = $"/{GlobalStaticConstants.Routes.API_CONTROLLER_NAME}/{GlobalStaticConstants.Routes.TOOLS_CONTROLLER_NAME}/{GlobalStaticConstants.Routes.FILE_CONTROLLER_NAME}-{GlobalStaticConstants.Routes.UPDATE_ACTION_NAME}";
        routeUri += $"?{GlobalStaticConstants.Routes.REMOTE_CONTROLLER_NAME}_{GlobalStaticConstants.Routes.DIRECTORY_CONTROLLER_NAME}={Convert.ToBase64String(Encoding.UTF8.GetBytes(remoteDirectory))}";

        HttpResponseMessage response = await httpClient.PostAsync(routeUri, form);

        response.EnsureSuccessStatusCode();
        httpClient.Dispose();
        string sd = response.Content.ReadAsStringAsync().Result;
        return JsonConvert.DeserializeObject<TResponseModel<string>>(sd)!;
    }
}