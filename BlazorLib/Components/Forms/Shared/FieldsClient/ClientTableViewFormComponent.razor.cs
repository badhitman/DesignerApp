using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using Microsoft.JSInterop;
using MudBlazor;
using SharedLib;

namespace BlazorLib.Components.Forms.Shared.FieldsClient;

public partial class ClientTableViewFormComponent : BlazorBusyComponentBaseModel
{
    [Inject]
    protected ILogger<ClientTableViewFormComponent> _logger { get; set; } = default!;

    [Inject]
    protected IDialogService _dialog_service { get; set; } = default!;

    [Inject]
    protected IJSRuntime _js_runtime { get; set; } = default!;

    [Inject]
    protected ISnackbar _snackbar { get; set; } = default!;

    [Inject]
    protected IFormsService _forms { get; set; } = default!;

    [Parameter]
    public string? Title { get; set; }

    [CascadingParameter, EditorRequired]
    public ConstructorFormQuestionnairePageJoinFormModelDB PageJoinForm { get; set; } = default!;

    [CascadingParameter]
    public ConstructorFormSessionModelDB? SessionQuestionnairie { get; set; }

    [CascadingParameter]
    public bool? InUse { get; set; }

    [CascadingParameter]
    public ConstructorFormQuestionnairePageModelDB? QuestionnairePage { get; set; }

    [CascadingParameter, EditorRequired]
    public ConstructorFormModelDB Form { get; set; } = default!;

    protected static bool IsReadonly(ClaimsPrincipal clp, ConstructorFormSessionModelDB sq)
    {
        string? email = clp.Claims.FirstOrDefault(x => x.Type.Equals(ClaimTypes.Email))?.Value;
        return !clp.Claims.Any(x => x.Type.Equals(ClaimTypes.Role, StringComparison.OrdinalIgnoreCase) && (x.Value.Equals("Admin", StringComparison.OrdinalIgnoreCase))) && sq.SessionStatus >= SessionsStatusesEnum.Sended && !sq.CreatorEmail.Equals(email, StringComparison.OrdinalIgnoreCase);
    }

    protected bool TableCalcKit { get; set; } = false;
    protected TableCalcKitComponent? _table_kit_ref;
    protected void OpenEditRowAction(uint row_num)
    {
        DialogParameters<ClientTableRowEditDialogComponent> parameters = new()
        {
            { x => x.RowNum, row_num },
            { x => x.SessionQuestionnairie, SessionQuestionnairie },
            { x => x.QuestionnairePage, QuestionnairePage },
            { x => x.PageJoinForm, PageJoinForm }
        };

        DialogOptions options = new() { MaxWidth = MaxWidth.ExtraExtraLarge, FullWidth = true, CloseOnEscapeKey = true };
        InvokeAsync(async () =>
        {
            DialogResult result = await _dialog_service.Show<ClientTableRowEditDialogComponent>($"Редактирование строки данных №:{row_num}", parameters, options).Result;
            await ReloadSession();
        });
    }

    protected void DeleteRowAction(uint row_num)
    {
        if (SessionQuestionnairie is null)
        {
            _snackbar.Add("SessionQuestionnairie is null. error {EAE6D2B8-7285-4F95-976A-7FA0C1F72ECF}", Severity.Error, c => c.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
            return;
        }

        IsBusyProgress = true;
        StateHasChanged();
        InvokeAsync(async () =>
        {
            ValueFieldSessionQuestionnaireBaseModel req = new() { GroupByRowNum = row_num, JoinFormId = PageJoinForm.Id, SessionId = SessionQuestionnairie.Id };
            ResponseBaseModel rest = await _forms.DeleteValuesFieldsByGroupSessionQuestionnairieByRowNum(req);
            IsBusyProgress = false;

            _snackbar.ShowMessagesResponse(rest.Messages);
            if (!rest.Success())
            {
                _snackbar.Add($"Ошибка {{E223CBC5-5BD5-4BEC-8A68-9601391BE10F}} Action: {rest.Message()}", Severity.Error, conf => conf.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
                return;
            }
            await ReloadSession();
        });
    }

    protected async Task AddRowToTable()
    {
        if (SessionQuestionnairie is null)
        {
            _snackbar.Add("SessionQuestionnairie is null. error {98E8A59C-BF72-462A-8894-29EF12245E63}", Severity.Error, c => c.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
            return;
        }

        FieldSessionQuestionnairieBaseModel row_obj = new()
        {
            JoinFormId = PageJoinForm.Id,
            SessionId = SessionQuestionnairie.Id
        };
        IsBusyProgress = true;
        CreateObjectOfIntKeyResponseModel rest = await _forms.AddRowToTable(row_obj);
        IsBusyProgress = false;

        _snackbar.ShowMessagesResponse(rest.Messages);
        if (!rest.Success())
        {
            _snackbar.Add($"Ошибка {{0F45F44B-900B-46CD-AC42-C866F8618A2E}} Action: {rest.Message()}", Severity.Error, conf => conf.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
            return;
        }
        uint row_num = (uint)rest.Id;
        DialogParameters<ClientTableRowEditDialogComponent> parameters = new()
        {
            { x => x.RowNum, row_num },
            { x => x.SessionQuestionnairie, SessionQuestionnairie },
            { x => x.QuestionnairePage, QuestionnairePage },
            { x => x.PageJoinForm, PageJoinForm }
        };
        DialogOptions options = new() { MaxWidth = MaxWidth.ExtraExtraLarge, FullWidth = true, CloseOnEscapeKey = true };
        DialogResult result = await _dialog_service.Show<ClientTableRowEditDialogComponent>($"Созданная строка данных №{rest.Id}", parameters, options).Result;
        ValueFieldSessionQuestionnaireBaseModel req = new()
        {
            GroupByRowNum = row_num,
            JoinFormId = PageJoinForm.Id,
            SessionId = SessionQuestionnairie.Id,
            IsSelf = true
        };
        _ = await _forms.DeleteValuesFieldsByGroupSessionQuestionnairieByRowNum(req);
        await ReloadSession();
    }

    async Task ReloadSession()
    {
        if (SessionQuestionnairie is null)
        {
            _snackbar.Add("SessionQuestionnairie is null. error {E4AE03AE-85A8-4D99-8C66-2FB46B84C67E}", Severity.Error, c => c.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
            return;
        }
        IsBusyProgress = true;
        FormSessionQuestionnairieResponseModel rest = string.IsNullOrWhiteSpace(SessionQuestionnairie.SessionToken)
        ? await _forms.GetSessionQuestionnairie(SessionQuestionnairie.Id)
        : await _forms.GetSessionQuestionnairie(SessionQuestionnairie.SessionToken);
        IsBusyProgress = false;

        _snackbar.ShowMessagesResponse(rest.Messages);
        if (!rest.Success())
        {
            _snackbar.Add($"Ошибка {{F046D8EB-DAA2-46AB-9C81-A43D5827BDA6}} Action: {rest.Message()}", Severity.Error, conf => conf.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
            return;
        }

        if (rest.SessionQuestionnairie is not null)
            SessionQuestionnairie.Reload(rest.SessionQuestionnairie);

        _table_kit_ref?.Update();

        StateHasChanged();
    }
}