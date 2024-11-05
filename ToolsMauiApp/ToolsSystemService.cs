////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using SharedLib;

namespace ToolsMauiApp;

/// <summary>
/// ToolsSystemService
/// </summary>
public class ToolsSystemService(IHttpClientFactory HttpClientFactory) : IToolsSystemService
{
    /// <inheritdoc/>
    public async Task<TResponseModel<ExpressProfileResponseModel>> GetMe()
    {
        using HttpClient client = HttpClientFactory.CreateClient(HttpClientsNamesEnum.Tools.ToString());
        return await client.GetStringAsync<ExpressProfileResponseModel>($"/{GlobalStaticConstants.Routes.API_CONTROLLER_NAME}/{GlobalStaticConstants.Routes.INFO_CONTROLLER_NAME}/{GlobalStaticConstants.Routes.MY_CONTROLLER_NAME}");
    }
}