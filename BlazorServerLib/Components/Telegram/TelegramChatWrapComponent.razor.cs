////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components;
using BlazorLib;
using SharedLib;
using MudBlazor;
using Microsoft.AspNetCore.Components.Forms;

namespace BlazorWebLib.Components.Telegram;

/// <summary>
/// TelegramChatWrapComponent
/// </summary>
public partial class TelegramChatWrapComponent : BlazorBusyComponentBaseModel
{
    [Inject]
    ITelegramRemoteTransmissionService TelegramRepo { get; set; } = default!;

    [Inject]
    ISnackbar SnackbarRepo { get; set; } = default!;


    /// <summary>
    /// Чат
    /// </summary>
    [Parameter, EditorRequired]
    public required ChatTelegramModelDB Chat { get; set; }

    /// <summary>
    /// SendMessageHandle
    /// </summary>
    [CascadingParameter]
    public Action<SendTextMessageTelegramBotModel>? SendMessageHandle { get; set; }


    private string _inputFileId = Guid.NewGuid().ToString();

    string? _textSendMessage;

    MessagesTelegramComponent _messagesTelegramComponent = default!;

    bool NavbarToggle = true;

    private readonly List<IBrowserFile> loadedFiles = [];

    async Task SendMessage()
    {
        if (string.IsNullOrWhiteSpace(_textSendMessage))
            throw new ArgumentNullException(nameof(_textSendMessage));

        IsBusyProgress = true;
        SendTextMessageTelegramBotModel req = new() { Message = _textSendMessage, UserTelegramId = Chat.ChatTelegramId, From = "Техподдержка" };

        // await file.OpenReadStream(maxFileSize).CopyToAsync(fs);
        MemoryStream ms;

        if (loadedFiles.Count != 0)
        {
            req.Files = [];

            foreach (var fileBrowser in loadedFiles)
            {
                ms = new();
                await fileBrowser.OpenReadStream().CopyToAsync(ms);
                req.Files.Add(new() { ContentType = fileBrowser.ContentType, Name = fileBrowser.Name, Data = ms.ToArray() });
                await ms.DisposeAsync();
            }
        }

        TResponseModel<MessageComplexIdsModel?> rest = await TelegramRepo.SendTextMessageTelegram(req);
        _textSendMessage = "";
        await _messagesTelegramComponent.TableRef.ReloadServerData();

        IsBusyProgress = false;
        SnackbarRepo.ShowMessagesResponse(rest.Messages);
        loadedFiles.Clear();
        _inputFileId = Guid.NewGuid().ToString();
        if (SendMessageHandle is not null)
            SendMessageHandle(req);
    }

    void SelectFilesChange(InputFileChangeEventArgs e)
    {
        loadedFiles.Clear();

        foreach (IBrowserFile file in e.GetMultipleFiles())
        {
            loadedFiles.Add(file);
        }
    }

    string GetTitle()
    {
        if (!string.IsNullOrWhiteSpace(Chat.Username))
            return $"@{Chat.Username}";

        if (!string.IsNullOrWhiteSpace(Chat.Title))
            return $"{Chat.Title} /{Chat.FirstName} {Chat.LastName}";

        if (!string.IsNullOrWhiteSpace(Chat.FirstName) || !string.IsNullOrWhiteSpace(Chat.LastName))
            return $"{Chat.FirstName} {Chat.LastName}";

        return $"#{Chat.ChatTelegramId}";
    }
}