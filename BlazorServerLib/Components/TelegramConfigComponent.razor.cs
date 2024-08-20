﻿////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using BlazorLib;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using SharedLib;

namespace BlazorWebLib.Components;

/// <summary>
/// TelegramConfigComponent
/// </summary>
public partial class TelegramConfigComponent : BlazorBusyComponentBaseModel
{
    [Inject]
    ISnackbar SnackbarRepo { get; set; } = default!;

    [Inject]
    ISerializeStorageRemoteTransmissionService SerializeStorageRepo { get; set; } = default!;

    bool _isCommandModeTelegramBot;
    bool IsCommandModeTelegramBot
    {
        get => _isCommandModeTelegramBot;
        set
        {
            _isCommandModeTelegramBot = value;
            InvokeAsync(SaveMode);
        }
    }

    async void SaveMode()
    {
        IsBusyProgress = true;
        TResponseModel<int> res = await SerializeStorageRepo.SaveParameter<bool?>(IsCommandModeTelegramBot, GlobalStaticConstants.CloudStorageMetadata.ParameterIsCommandModeTelegramBot);
        IsBusyProgress = false;
        SnackbarRepo.ShowMessagesResponse(res.Messages);
    }

    /// <inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
        IsBusyProgress = true;
        TResponseModel<bool?> res_IsCommandModeTelegramBot = await SerializeStorageRepo.ReadParameter<bool?>(GlobalStaticConstants.CloudStorageMetadata.ParameterIsCommandModeTelegramBot);
        IsBusyProgress = false;
        SnackbarRepo.ShowMessagesResponse(res_IsCommandModeTelegramBot.Messages);
        _isCommandModeTelegramBot = res_IsCommandModeTelegramBot.Response == true;
    }
}