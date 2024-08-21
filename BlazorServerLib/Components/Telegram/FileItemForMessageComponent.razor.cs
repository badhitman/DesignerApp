////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using BlazorLib;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using SharedLib;
using System.IO;

namespace BlazorWebLib.Components.Telegram;

public partial class FileItemForMessageComponent : BlazorBusyComponentBaseModel
{
    [Inject]
    ITelegramRemoteTransmissionService telegramRepo { get; set; } = default!;

    [Inject]
    ISnackbar SnackbarRepo { get; set; } = default!;

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
        IsBusyProgress = true;
        TResponseModel<byte[]?> rest = await telegramRepo.GetFile(FileElement.FileId);
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