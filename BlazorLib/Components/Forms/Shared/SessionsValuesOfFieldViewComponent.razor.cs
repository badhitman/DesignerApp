using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using MudBlazor;
using SharedLib;

namespace BlazorLib.Components.Forms.Shared;

public partial class SessionsValuesOfFieldViewComponent : BlazorBusyComponentBaseModel
{
    [Inject]
    protected ILogger<SessionsValuesOfFieldViewComponent> _logger { get; set; } = default!;

    [Inject]
    protected IJSRuntime JsRuntimeRepo { get; set; } = default!;

    [Inject]
    protected ISnackbar SnackbarRepo { get; set; } = default!;

    [Inject]
    protected IFormsService FormsRepo { get; set; } = default!;


    [CascadingParameter, EditorRequired]
    public ConstructorFormModelDB Form { get; set; } = default!;

    [Parameter, EditorRequired]
    public string FieldName { get; set; } = default!;

    [Parameter, EditorRequired]
    public Action<IEnumerable<EntryDictModel>> ShowReferalsHandler { get; set; } = default!;

    public async Task FindFields()
    {
        IsBusyProgress = true;
        EntriesDictResponseModel rest = await FormsRepo.FindSessionsQuestionnairesByFormFieldName(new() { FormId = Form.Id, FieldName = FieldName });
        IsBusyProgress = false;
        //SnackbarRepo.ShowMessagesResponse(rest.Content.Messages);
        if (!rest.Success())
        {
            SnackbarRepo.Add($"Ошибка {{8425BABE-0EAF-44CC-925D-DBB5824EB1F3}} Action: {rest.Message()}", Severity.Error, conf => conf.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
            return;
        }
        if (rest.Elements is not null)
            ShowReferalsHandler(rest.Elements);
    }
}