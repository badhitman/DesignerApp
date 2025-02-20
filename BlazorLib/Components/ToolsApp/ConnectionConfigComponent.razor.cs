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

    [Inject]
    IClientHTTPRestService RestClientRepo { get; set; } = default!;


    /// <inheritdoc/>
    [Parameter, EditorRequired]
    public required ToolsAppMainComponent Parent { get; set; }


    /// <summary>
    /// Форма подключения заполнена?
    /// </summary>
    bool ValidateForm => !string.IsNullOrWhiteSpace(TokenAccess) &&
        !string.IsNullOrWhiteSpace(AddressBaseUri);

    /// <summary>
    /// Форма изменена?
    /// </summary>
    public bool IsEdited => ApiConnect.Name != Name || ApiConnect.TokenAccess != TokenAccess || ApiConnect.AddressBaseUri != AddressBaseUri;

    /// <summary>
    /// Форма может быть сохранена?
    /// </summary>
    /// <remarks>
    /// Если форма изменена и корректно заполнена
    /// </remarks>
    bool CanSave => IsEdited && ValidateForm;


    string name = string.Empty;
    string Name
    {
        get => name;
        set
        {
            name = value;
        }
    }

    string tokenAccess = string.Empty;
    string TokenAccess
    {
        get => tokenAccess;
        set
        {
            tokenAccess = value;
        }
    }

    string addressBaseUri = string.Empty;
    string AddressBaseUri
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

    /// <summary>
    /// getMe
    /// </summary>
    public TResponseModel<ExpressProfileResponseModel>? GetMe { get; set; }

    CancellationTokenSource cancelTokenSource = new();
    CancellationToken? token;

    /// <summary>
    /// Проверить подключение
    /// </summary>
    /// <param name="testForm">Если требуется проверить настройки из формы</param>
    public async Task TestConnect(bool testForm = false)
    {
        await SetBusy();
        ApiRestConfigModelDB? backupConf = null;
        if (testForm)
        {
            backupConf = GlobalTools.CreateDeepCopy(ApiConnect);

            ApiConnect.AddressBaseUri = AddressBaseUri;
            ApiConnect.HeaderName = HeaderName;
            ApiConnect.TokenAccess = TokenAccess;
        }
        token = cancelTokenSource.Token;
        GetMe = await RestClientRepo.GetMe(token.Value);

        if (backupConf is not null)
            ApiConnect.Update(backupConf);

        await SetBusy(false);
        SnackbarRepo.ShowMessagesResponse(GetMe.Messages);

        Parent.StateHasChangedCall();
    }

    async Task SaveToken()
    {
        ApiRestConfigModelDB req = new()
        {
            Id = ApiConnect.Id,
            Name = Name,
            AddressBaseUri = AddressBaseUri,
            TokenAccess = TokenAccess,
            HeaderName = HeaderName,
        };
        await SetBusy();
        TResponseModel<int> res = await ToolsApp.UpdateOrCreateConfig(req);
        SnackbarRepo.ShowMessagesResponse(res.Messages);
        await SetBusy(false);
        if (res.Success())
            await Parent.SetActiveHandler(req.Id > 0 ? req.Id : res.Response);
    }

    /// <summary>
    /// ResetForm
    /// </summary>
    public void ResetForm()
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