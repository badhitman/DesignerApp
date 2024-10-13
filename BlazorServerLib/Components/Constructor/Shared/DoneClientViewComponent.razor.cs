////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using BlazorLib;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using SharedLib;

namespace BlazorWebLib.Components.Constructor.Shared;

/// <summary>
/// Done client view
/// </summary>
public partial class DoneClientViewComponent : BlazorBusyComponentBaseModel
{
    /// <inheritdoc/>
    [Inject]
    protected ISnackbar SnackbarRepo { get; set; } = default!;

    /// <inheritdoc/>
    [Inject]
    protected IConstructorRemoteTransmissionService ConstructorRepo { get; set; } = default!;


    /// <inheritdoc/>
    [CascadingParameter]
    public required SessionOfDocumentDataModelDB SessionDocument { get; set; }

    /// <inheritdoc/>
    [CascadingParameter]
    public required bool InUse { get; set; }


    bool InitSend = false;

    /// <inheritdoc/>
    protected async Task SetAsDone()
    {
        if (string.IsNullOrWhiteSpace(SessionDocument.SessionToken))
        {
            SnackbarRepo.Add("string.IsNullOrWhiteSpace(SessionDocument.SessionToken). error 5E2D7979-53E7-4130-8DF2-53C00D378BEA", Severity.Error, c => c.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
            return;
        }

        if (!InitSend)
        {
            InitSend = true;
            return;
        }

        IsBusyProgress = true;
        ResponseBaseModel rest = await ConstructorRepo.SetDoneSessionDocumentData(SessionDocument.SessionToken);
        IsBusyProgress = false;

        SnackbarRepo.ShowMessagesResponse(rest.Messages);
        if (rest.Success())
            SessionDocument.SessionStatus = SessionsStatusesEnum.Sended;
    }
}