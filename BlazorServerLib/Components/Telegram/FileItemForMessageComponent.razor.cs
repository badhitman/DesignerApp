////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using BlazorLib;
using MudBlazor;
using SharedLib;

namespace BlazorWebLib.Components.Telegram;

/// <summary>
/// FileItemForMessageComponent
/// </summary>
public partial class FileItemForMessageComponent : BlazorBusyComponentBaseModel
{
    [Inject]
    ITelegramRemoteTransmissionService telegramRepo { get; set; } = default!;

    [Inject]
    IJSRuntime JSRepo { get; set; } = default!;


    /// <summary>
    /// FileElement
    /// </summary>
    [Parameter, EditorRequired]
    public required FileBaseTelegramModel FileElement { get; set; }


    string? fileName;

    async Task DownloadFile()
    {
        await SetBusy();
        TResponseModel<byte[]> rest = await telegramRepo.GetFile(FileElement.FileId);
        IsBusyProgress = false;
        SnackbarRepo.ShowMessagesResponse(rest.Messages);
        if (rest.Response is not null)
        {
            MemoryStream ms = new(rest.Response);
            using var streamRef = new DotNetStreamReference(stream: ms);
            await JSRepo.InvokeVoidAsync("downloadFileFromStream", fileName, streamRef);
        }
    }
}