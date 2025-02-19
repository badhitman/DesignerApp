////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components;
using SharedLib;

namespace BlazorLib.Components.ToolsApp;

/// <summary>
/// ConnectionConfigComponent
/// </summary>
public partial class ConnectionConfigComponent : BlazorBusyComponentBaseModel
{
    [Inject]
    ApiRestConfigModelDB ApiConnect { get; set; } = default!;

    [Inject]
    IToolsAppManager ToolsApp { get; set; } = default!;


    /// <inheritdoc/>
    [Parameter, EditorRequired]
    public required Action<int> SetActiveHandle { get; set; }


    bool ValidateForm => !string.IsNullOrWhiteSpace(TokenAccess) &&
        !string.IsNullOrWhiteSpace(AddressBaseUri);

    bool CanSave => IsEdited && ValidateForm;

    bool IsEdited => ApiConnect.Name != Name || ApiConnect.TokenAccess != TokenAccess || ApiConnect.AddressBaseUri != AddressBaseUri;


    string name = default!;
    string Name
    {
        get => name;
        set
        {
            name = value;
        }
    }

    string? tokenAccess;
    string? TokenAccess
    {
        get => tokenAccess;
        set
        {
            tokenAccess = value;
        }
    }

    string? addressBaseUri;
    string? AddressBaseUri
    {
        get => addressBaseUri;
        set
        {
            addressBaseUri = value;
        }
    }

    string headerName = "token-access";
    string HeaderName
    {
        get => headerName;
        set
        {
            headerName = value;
        }
    }

    async Task TestConnect()
    {
        await SetBusy();

        await SetBusy(false);
    }

    async Task SaveToken()
    {
        ApiRestConfigModelDB req = new()
        {
            Name = Name,
            AddressBaseUri = AddressBaseUri,
            TokenAccess = TokenAccess,
            HeaderName = HeaderName,
        };
        await SetBusy();
        ResponseBaseModel res = await ToolsApp.UpdateOrCreateConfig(req);
        SnackbarRepo.ShowMessagesResponse(res.Messages);
        await SetBusy(false);
    }

    void ResetForm()
    {
        name = ApiConnect.Name;
        tokenAccess = ApiConnect.TokenAccess;
        addressBaseUri = ApiConnect.AddressBaseUri;
        headerName = ApiConnect.HeaderName;
    }

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        base.OnInitialized();
        ResetForm();
    }
}