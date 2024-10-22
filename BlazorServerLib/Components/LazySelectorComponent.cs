////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using BlazorLib;
using MudBlazor;
using Microsoft.Extensions.Logging;

namespace BlazorWebLib.Components;

/// <summary>
/// LazySelectorComponent
/// </summary>
public abstract class LazySelectorComponent<TRow> : BlazorBusyComponentBaseModel
{
    /// <summary>
    /// JS
    /// </summary>
    [Inject]
    protected IJSRuntime JS { get; set; } = default!;

    /// <summary>
    /// IsReadOnly
    /// </summary>
    [Parameter]
    public bool IsReadOnly { get; set; }

    /// <summary>
    /// Поддержка NULL выбора в селекторе, указывающий на то что ни чего не выбрано
    /// </summary>
    [Parameter]
    public bool NullableAllow { get; set; }

    /// <summary>
    /// SelectChatHandle
    /// </summary>
    [Parameter, EditorRequired]
    public required Action<TRow?> SelectHandleAction { get; set; }

    /// <summary>
    /// SetHeightCard
    /// </summary>
    [Parameter]
    public required Action<double>? SetHeightCard { get; set; }


    /// <summary>
    /// PageNum
    /// </summary>
    protected int PageNum = 0;
    /// <summary>
    /// TotalRowsCount
    /// </summary>
    protected int TotalRowsCount;

    /// <summary>
    /// LoadedData
    /// </summary>
    public readonly List<TRow> LoadedData = [];

    /// <summary>
    /// IsEditing
    /// </summary>
    protected bool IsEditing { get; set; }

    /// <summary>
    /// LoadPartData
    /// </summary>
    public abstract Task LoadPartData();

    /// <summary>
    /// SelectedObject
    /// </summary>
    public TRow? SelectedObject { get; set; }

    /// <summary>
    /// _selectedValueText
    /// </summary>
    protected string? _selectedValueText;

    /// <summary>
    /// SelectedValueText
    /// </summary>
    protected string? SelectedValueText
    {
        get => IsEditing ? _selectedValueText : SelectedObject?.ToString();
        set
        {
            _selectedValueText = value;

            LoadedData.Clear();
            PageNum = 0;
            TotalRowsCount = 0;
            InvokeAsync(LoadPartData);
        }
    }

    /// <inheritdoc/>
    protected string StyleIfEditing => IsEditing ? $"position: absolute; inset: 0px auto auto 0px; margin: 0px; transform: translate(0px, {HeightToggleBtn}px);" : "";
    /// <inheritdoc/>
    protected string ShowIfEditing => IsEditing ? " show" : "";

    /// <inheritdoc/>
    protected double HeightToggleBtn, yToggleBtn, HeightDropdown;
    /// <inheritdoc/>
    protected double _HeightToggleBtn, _yToggleBtn, _HeightDropdown;

    /// <inheritdoc/>
    protected static readonly int page_size = 10;
    /// <inheritdoc/>
    protected readonly string toggleBtnId = Guid.NewGuid().ToString();
    /// <inheritdoc/>
    protected readonly string dropdownId = Guid.NewGuid().ToString();

    /// <inheritdoc/>
    protected void SelectElementEvent(TRow sender)
    {
        SelectedObject = sender;
        if (SelectHandleAction is not null)
            SelectHandleAction(sender);

        EditToggle();
    }

    bool _ised;
    /// <inheritdoc/>
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        try
        {
            HeightToggleBtn = await JS.InvokeAsync<int>("BoundingClientRect.Height", toggleBtnId);
        }
        catch
        {

            return;
        }

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

    /// <summary>
    /// EditToggle
    /// </summary>
    public void EditToggle()
    {
        IsEditing = !IsEditing;
        if (!IsEditing)
        {
            LoadedData.Clear();
            PageNum = 0;
            TotalRowsCount = 0;
            return;
        }
        _selectedValueText = "";
        InvokeAsync(LoadPartData);
    }
}