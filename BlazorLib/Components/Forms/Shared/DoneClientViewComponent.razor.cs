using Microsoft.AspNetCore.Components;
using MudBlazor;
using SharedLib;

namespace BlazorLib.Components.Forms.Shared;

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
    protected IFormsService FormsRepo { get; set; } = default!;

    /// <inheritdoc/>
    [CascadingParameter]
    public required ConstructorFormSessionModelDB SessionQuestionnaire { get; set; }

    /// <inheritdoc/>
    [CascadingParameter]
    public required bool InUse { get; set; }

    bool InitSend = false;

    /// <inheritdoc/>
    protected async Task SetAsDone()
    {
        if (string.IsNullOrWhiteSpace(SessionQuestionnaire.SessionToken))
        {
            SnackbarRepo.Add("string.IsNullOrWhiteSpace(SessionQuestionnaire.SessionToken). error {0F19A8A7-A486-49B9-857B-C45CFC904E9D}", Severity.Error, c => c.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
            return;
        }

        if (!InitSend)
        {
            InitSend = true;
            return;
        }

        IsBusyProgress = true;
        ResponseBaseModel rest = await FormsRepo.SetDoneSessionQuestionnaire(SessionQuestionnaire.SessionToken);
        IsBusyProgress = false;

        SnackbarRepo.ShowMessagesResponse(rest.Messages);
        if (rest.Success())
            SessionQuestionnaire.SessionStatus = SessionsStatusesEnum.Sended;
    }
}