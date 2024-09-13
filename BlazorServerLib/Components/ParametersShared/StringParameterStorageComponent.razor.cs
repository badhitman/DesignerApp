////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components;
using BlazorLib;
using MudBlazor;
using SharedLib;

namespace BlazorWebLib.Components.ParametersShared;

/// <summary>
/// StringParameterStorageComponent
/// </summary>
public partial class StringParameterStorageComponent : BlazorBusyComponentBaseModel
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


    string? _textValue;
    string? TextValue
    {
        get => _textValue;
        set
        {
            _textValue = value;
            InvokeAsync(async () => { await StoreRepo.SaveParameter(_textValue, KeyStorage); });
        }
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