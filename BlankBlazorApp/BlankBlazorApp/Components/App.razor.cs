using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using MudBlazor;
using SharedLib;

namespace BlankBlazorApp.Components;

/// <summary>
/// App
/// </summary>
public partial class App
{
    [Inject]
    IStorageTransmission StoreRepo { get; set; } = default!;

    [Inject]
    ITelegramTransmission TgRemoteCall { get; set; } = default!;

    [Inject]
    IOptions<TelegramBotConfigModel> WebConfig { get; set; } = default!;

    [Inject]
    NavigationManager NavigatorRepo { get; set; } = default!;

    [Inject]
    ISnackbar SnackbarRepo { get; set; } = default!;


    static bool _isLoaded = false;
    static bool _includeTelegramBotWeAppScript = false;

    /// <inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
        if (WebConfig.Value.BaseUri is null)
            WebConfig.Value.BaseUri = NavigatorRepo.BaseUri;

        if (!_isLoaded)
        {
            List<Task> _tasks = [Task.Run(async () => {
                TResponseModel<bool> tgWebAppInclude = await StoreRepo.ReadParameter<bool>(GlobalStaticConstants.CloudStorageMetadata.ParameterIncludeTelegramBotWebApp);
                _includeTelegramBotWeAppScript = tgWebAppInclude.Success() && tgWebAppInclude.Response == true;
            })];
            _isLoaded = true;
            if (ServiceProviderExtensions.SetRemoteConf?.Success() != true)
            {
                _tasks.AddRange([
                    TgRemoteCall.SetWebConfigTelegram(WebConfig.Value, false),
                    TgRemoteCall.SetWebConfigHelpdesk(WebConfig.Value, false),
                    TgRemoteCall.SetWebConfigStorage(WebConfig.Value, false)]);

                await Task.WhenAll(_tasks);
            }

        }

    }
}
