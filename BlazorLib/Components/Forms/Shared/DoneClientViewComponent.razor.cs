﻿using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using MudBlazor;
using SharedLib;

namespace BlazorLib.Components.Forms.Shared;

public partial class DoneClientViewComponent : BlazorBusyComponentBaseModel
{
    [Inject]
    protected ILogger<DoneClientViewComponent> _logger { get; set; } = default!;

    [Inject]
    protected ISnackbar _snackbar { get; set; } = default!;

    [Inject]
    protected IFormsService _forms { get; set; } = default!;

    [CascadingParameter, EditorRequired]
    public ConstructorFormSessionModelDB SessionQuestionnairie { get; set; } = default!;

    [CascadingParameter, EditorRequired]
    public bool InUse { get; set; } = default!;

    bool InitSend = false;

    protected async Task SetAsDone()
    {
        if (string.IsNullOrWhiteSpace(SessionQuestionnairie.SessionToken))
        {
            _snackbar.Add("string.IsNullOrWhiteSpace(SessionQuestionnairie.SessionToken). error {0F19A8A7-A486-49B9-857B-C45CFC904E9D}", Severity.Error, c => c.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
            return;
        }

        if (!InitSend)
        {
            InitSend = true;
            return;
        }

        IsBusyProgress = true;
        ResponseBaseModel rest = await _forms.SetDoneSessionQuestionnairie(SessionQuestionnairie.SessionToken);
        IsBusyProgress = false;

        _snackbar.ShowMessagesResponse(rest.Messages);
        if (rest.Success())
            SessionQuestionnairie.SessionStatus = SessionsStatusesEnum.Sended;
    }
}