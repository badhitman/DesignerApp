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
/// ChatSelectInputComponent
/// </summary>
public partial class ChatSelectInputComponent : BlazorBusyComponentBaseModel
{
    [Inject]
    ITelegramRemoteTransmissionService TelegramRepo { get; set; } = default!;

    [Inject]
    ISnackbar SnackbarRepo { get; set; } = default!;

    [Inject]
    IJSRuntime JS { get; set; } = default!;


    /// <summary>
    /// Selected chat
    /// </summary>
    [Parameter, EditorRequired]
    public required long SelectedChat { get; set; }

    /// <summary>
    /// Поддержка NULL выбора в селекторе, указывающий на то что ни чего не выбрано
    /// </summary>
    [Parameter]
    public bool NullableAllow { get; set; }

    /// <summary>
    /// SelectChatHandle
    /// </summary>
    [Parameter, EditorRequired]
    public required Action<ChatTelegramModelDB?> SelectChatHandle { get; set; }

    /// <summary>
    /// SetHeightCard
    /// </summary>
    [Parameter]
    public required Action<double>? SetHeightCard { get; set; }


    void SelectChatEvent(ChatTelegramModelDB sender)
    {
        selectedChatDb = sender;
        if (SelectChatHandle is not null)
            SelectChatHandle(sender);

        EditToggle();
    }


    static readonly int page_size = 10;

    ChatTelegramModelDB selectedChatDb = default!;

    readonly string toggleBtnId = Guid.NewGuid().ToString();
    readonly string dropdownId = Guid.NewGuid().ToString();

    string StyleIfEditing => IsEditing ? $"position: absolute; inset: 0px auto auto 0px; margin: 0px; transform: translate(0px, {HeightToggleBtn}px);" : "";
    string ShowIfEditing => IsEditing ? " show" : "";
    bool IsEditing;

    readonly List<ChatTelegramModelDB> loadedData = [];
    int pageNum = 0;
    int TotalRowsCount;

    void EditToggle()
    {
        IsEditing = !IsEditing;
        if (!IsEditing)
        {
            loadedData.Clear();
            pageNum = 0;
            TotalRowsCount = 0;
            return;
        }
        _selectedChatText = "";
        InvokeAsync(LoadPartData);
    }

    async Task LoadPartData()
    {
        IsBusyProgress = true;
        TResponseModel<TPaginationResponseModel<ChatTelegramModelDB>?> rest = await TelegramRepo
            .ChatsSelect(new()
            {
                Payload = SelectedChatText,
                PageNum = pageNum,
                PageSize = page_size,
            });
        IsBusyProgress = false;
        SnackbarRepo.ShowMessagesResponse(rest.Messages);
        if (rest.Success() && rest.Response?.Response is not null)
        {
            TotalRowsCount = rest.Response.TotalRowsCount;
            loadedData.AddRange(rest.Response.Response);

            if (pageNum == 0)
                loadedData.Insert(0, new() { Title = "OFF" });

            pageNum++;
        }
        StateHasChanged();
    }

    string? _selectedChatText;
    string? SelectedChatText
    {
        get => IsEditing ? _selectedChatText : selectedChatDb.ToString();
        set
        {
            _selectedChatText = value;

            loadedData.Clear();
            pageNum = 0;
            TotalRowsCount = 0;
            InvokeAsync(LoadPartData);
        }
    }

    double HeightToggleBtn, yToggleBtn, HeightDropdown;
    double _HeightToggleBtn, _yToggleBtn, _HeightDropdown;
    bool _ised;

    /// <inheritdoc/>
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        HeightToggleBtn = await JS.InvokeAsync<int>("BoundingClientRect.Height", toggleBtnId);

        if (SetHeightCard is not null)
        {
            yToggleBtn = await JS.InvokeAsync<double>("BoundingClientRect.Y", toggleBtnId);
            HeightDropdown = await JS.InvokeAsync<double>("BoundingClientRect.Height", dropdownId);

            if (_HeightToggleBtn != HeightToggleBtn || _yToggleBtn != yToggleBtn || _HeightDropdown != HeightDropdown || _ised != IsEditing)
            {
                _HeightToggleBtn = HeightToggleBtn;
                _yToggleBtn = yToggleBtn;
                _HeightDropdown = HeightDropdown;
                _ised = IsEditing;

                SetHeightCard(IsEditing ? yToggleBtn + HeightToggleBtn + HeightDropdown : 0);
            }
        }
    }

    /// <inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
        if (SelectedChat < 1)
        {
            selectedChatDb = new() { Title = "OFF", Type = ChatsTypesTelegramEnum.Private };
            SelectChatHandle(selectedChatDb);
            return;
        }

        IsBusyProgress = true;
        TResponseModel<ChatTelegramModelDB[]?> rest = await TelegramRepo.ChatsReadTelegram([SelectedChat]);
        IsBusyProgress = false;
        SnackbarRepo.ShowMessagesResponse(rest.Messages);
        if (rest.Response is null || rest.Response.Length == 0)
        {
            SnackbarRepo.Error($"Не найден запрашиваемый чат #{SelectedChat}");
            return;
        }
        selectedChatDb = rest.Response.Single();
        _selectedChatText = selectedChatDb.ToString();
        SelectChatHandle(selectedChatDb);
    }
}