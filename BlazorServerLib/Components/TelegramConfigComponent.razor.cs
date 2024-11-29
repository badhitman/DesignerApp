////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components;
using BlazorLib;
using MudBlazor;
using SharedLib;

namespace BlazorWebLib.Components;

/// <summary>
/// TelegramConfigComponent
/// </summary>
public partial class TelegramConfigComponent : BlazorBusyComponentBaseModel
{
    [Inject]
    ISerializeStorageRemoteTransmissionService SerializeStorageRepo { get; set; } = default!;


    bool _isCommandModeTelegramBot;

    /// <inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
        await SetBusy();
        TResponseModel<bool?> res_IsCommandModeTelegramBot = await SerializeStorageRepo.ReadParameter<bool?>(GlobalStaticConstants.CloudStorageMetadata.ParameterIsCommandModeTelegramBot);
        IsBusyProgress = false;

        if (!res_IsCommandModeTelegramBot.Success())
            SnackbarRepo.ShowMessagesResponse(res_IsCommandModeTelegramBot.Messages);

        _isCommandModeTelegramBot = res_IsCommandModeTelegramBot.Response == true;
    }
}