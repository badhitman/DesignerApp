////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components;
using BlazorLib;
using SharedLib;

namespace BlazorWebLib.Components.ParametersShared;

/// <summary>
/// Bool simple storage component
/// </summary>
public partial class BoolSimpleStorageComponent : BlazorBusyComponentBaseModel
{
    [Inject]
    ISerializeStorageRemoteTransmissionService StorageTransmissionRepo { get; set; } = default!;


    /// <summary>
    /// Title
    /// </summary>
    [Parameter, EditorRequired]
    public required string Title { get; set; }

    /// <summary>
    /// Title
    /// </summary>
    [Parameter, EditorRequired]
    public required string Label { get; set; }

    /// <summary>
    /// Storage metadata
    /// </summary>
    [Parameter, EditorRequired]
    public required StorageMetadataModel StorageMetadata { get; set; }

    /// <summary>
    /// Hint for true
    /// </summary>
    [Parameter]
    public string? HintTrue { get; set; }

    /// <summary>
    /// Hint for false
    /// </summary>
    [Parameter]
    public string? HintFalse { get; set; }


    bool _storeValue;
    bool StoreValue
    {
        get => _storeValue;
        set
        {
            _storeValue = value;
            InvokeAsync(SaveParameter);
        }
    }

    /// <inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
        await SetBusy();
        TResponseModel<bool?> showCreatingIssue = await StorageTransmissionRepo.ReadParameter<bool?>(StorageMetadata);
        _storeValue = showCreatingIssue.Success() && showCreatingIssue.Response == true;
        await SetBusy(false);
    }

    async void SaveParameter()
    {
        await SetBusy();
        TResponseModel<int> res = await StorageTransmissionRepo.SaveParameter<bool?>(StoreValue, StorageMetadata, true);
        await SetBusy(false);
        SnackbarRepo.ShowMessagesResponse(res.Messages);
    }
}