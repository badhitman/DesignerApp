////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using BlazorLib;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using SharedLib;

namespace BlazorWebLib.Components.ParametersShared;

/// <summary>
/// CKEditorParameterStorageComponent
/// </summary>
public partial class CKEditorParameterStorageComponent : BlazorBusyComponentBaseModel
{
    [Inject]
    ISerializeStorageRemoteTransmissionService StoreRepo { get; set; } = default!;

    [Inject]
    ISnackbar SnackbarRepo { get; set; } = default!;


    /// <summary>
    /// Label
    /// </summary>
    [Parameter, EditorRequired]
    public required string Label { get; set; }

    /// <summary>
    /// KeyStorage
    /// </summary>
    [Parameter, EditorRequired]
    public required StorageCloudParameterModel KeyStorage { get; set; }

    /// <summary>
    /// HelperText
    /// </summary>
    [Parameter]
    public string? HelperText { get; set; }


    InputRichTextComponent _currentTemplateInputRichText_ref = default!;

    string? _textValue;
    string? TextValue
    {
        get => _textValue;
        set
        {
            _textValue = value;
            InvokeAsync(StoreData);
        }
    }

    async Task StoreData()
    {
        IsBusyProgress = true;
        await Task.Delay(1);
        await StoreRepo.SaveParameter(_textValue, KeyStorage);
        IsBusyProgress = false;
        StateHasChanged();
    }

    /// <inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
        IsBusyProgress = true;
        TResponseModel<string?> res = await StoreRepo.ReadParameter<string?>(KeyStorage);
        IsBusyProgress = false;
        SnackbarRepo.ShowMessagesResponse(res.Messages);
        _textValue = res.Response;
    }
}