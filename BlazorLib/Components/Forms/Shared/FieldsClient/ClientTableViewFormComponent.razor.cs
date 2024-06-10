using Microsoft.AspNetCore.Components;
using System.Security.Claims;
using MudBlazor;
using SharedLib;

namespace BlazorLib.Components.Forms.Shared.FieldsClient;

/// <summary>
/// Client table view form
/// </summary>
public partial class ClientTableViewFormComponent : BlazorBusyComponentBaseModel
{
    /// <inheritdoc/>
    [Inject]
    protected IDialogService DialogServiceRepo { get; set; } = default!;

    /// <inheritdoc/>
    [Inject]
    protected ISnackbar SnackbarRepo { get; set; } = default!;

    /// <inheritdoc/>
    [Inject]
    protected IFormsService FormsRepo { get; set; } = default!;

    /// <inheritdoc/>
    [Parameter]
    public string? Title { get; set; }

    /// <inheritdoc/>
    [CascadingParameter, EditorRequired]
    public required ConstructorFormQuestionnairePageJoinFormModelDB PageJoinForm { get; set; }

    /// <inheritdoc/>
    [CascadingParameter]
    public ConstructorFormSessionModelDB? SessionQuestionnaire { get; set; }

    /// <inheritdoc/>
    [CascadingParameter]
    public bool? InUse { get; set; }

    /// <inheritdoc/>
    [CascadingParameter]
    public ConstructorFormQuestionnairePageModelDB? QuestionnairePage { get; set; }

    /// <inheritdoc/>
    [CascadingParameter, EditorRequired]
    public required ConstructorFormModelDB Form { get; set; }

    /// <inheritdoc/>
    protected static bool IsReadonly(ClaimsPrincipal clp, ConstructorFormSessionModelDB sq)
    {
        string? email = clp.Claims.FirstOrDefault(x => x.Type.Equals(ClaimTypes.Email))?.Value;
        return !clp.Claims.Any(x => x.Type.Equals(ClaimTypes.Role, StringComparison.OrdinalIgnoreCase) && (x.Value.Equals("Admin", StringComparison.OrdinalIgnoreCase))) && sq.SessionStatus >= SessionsStatusesEnum.Sended && !sq.CreatorEmail.Equals(email, StringComparison.OrdinalIgnoreCase);
    }

    /// <inheritdoc/>
    protected bool TableCalculationKit { get; set; } = false;
    
    /// <inheritdoc/>
    protected TableCalculationKitComponent? _table_kit_ref;
    
    /// <inheritdoc/>
    protected void OpenEditRowAction(uint row_num)
    {
        DialogParameters<ClientTableRowEditDialogComponent> parameters = new()
        {
            { x => x.RowNum, row_num },
            { x => x.SessionQuestionnaire, SessionQuestionnaire },
            { x => x.QuestionnairePage, QuestionnairePage },
            { x => x.PageJoinForm, PageJoinForm }
        };

        DialogOptions options = new() { MaxWidth = MaxWidth.ExtraExtraLarge, FullWidth = true, CloseOnEscapeKey = true };
        _ = InvokeAsync(async () =>
        {
            DialogResult result = await DialogServiceRepo.Show<ClientTableRowEditDialogComponent>($"Редактирование строки данных №:{row_num}", parameters, options).Result;
            await ReloadSession();
        });
    }

    /// <inheritdoc/>
    protected void DeleteRowAction(uint row_num)
    {
        if (SessionQuestionnaire is null)
        {
            SnackbarRepo.Add("SessionQuestionnaire is null. error {EAE6D2B8-7285-4F95-976A-7FA0C1F72ECF}", Severity.Error, c => c.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
            return;
        }

        IsBusyProgress = true;
        StateHasChanged();
        _ = InvokeAsync(async () =>
        {
            ValueFieldSessionQuestionnaireBaseModel req = new() { GroupByRowNum = row_num, JoinFormId = PageJoinForm.Id, SessionId = SessionQuestionnaire.Id };
            ResponseBaseModel rest = await FormsRepo.DeleteValuesFieldsByGroupSessionQuestionnaireByRowNum(req);
            IsBusyProgress = false;

            SnackbarRepo.ShowMessagesResponse(rest.Messages);
            if (!rest.Success())
            {
                SnackbarRepo.Add($"Ошибка {{E223CBC5-5BD5-4BEC-8A68-9601391BE10F}} Action: {rest.Message()}", Severity.Error, conf => conf.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
                return;
            }
            await ReloadSession();
        });
    }

    /// <inheritdoc/>
    protected async Task AddRowToTable()
    {
        if (SessionQuestionnaire is null)
        {
            SnackbarRepo.Add("SessionQuestionnaire is null. error {98E8A59C-BF72-462A-8894-29EF12245E63}", Severity.Error, c => c.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
            return;
        }

        FieldSessionQuestionnaireBaseModel row_obj = new()
        {
            JoinFormId = PageJoinForm.Id,
            SessionId = SessionQuestionnaire.Id
        };
        IsBusyProgress = true;
        CreateObjectOfIntKeyResponseModel rest = await FormsRepo.AddRowToTable(row_obj);
        IsBusyProgress = false;

        SnackbarRepo.ShowMessagesResponse(rest.Messages);
        if (!rest.Success())
        {
            SnackbarRepo.Add($"Ошибка {{0F45F44B-900B-46CD-AC42-C866F8618A2E}} Action: {rest.Message()}", Severity.Error, conf => conf.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
            return;
        }
        uint row_num = (uint)rest.Id;
        DialogParameters<ClientTableRowEditDialogComponent> parameters = new()
        {
            { x => x.RowNum, row_num },
            { x => x.SessionQuestionnaire, SessionQuestionnaire },
            { x => x.QuestionnairePage, QuestionnairePage },
            { x => x.PageJoinForm, PageJoinForm }
        };
        DialogOptions options = new() { MaxWidth = MaxWidth.ExtraExtraLarge, FullWidth = true, CloseOnEscapeKey = true };
        DialogResult result = await DialogServiceRepo.Show<ClientTableRowEditDialogComponent>($"Созданная строка данных №{rest.Id}", parameters, options).Result;
        ValueFieldSessionQuestionnaireBaseModel req = new()
        {
            GroupByRowNum = row_num,
            JoinFormId = PageJoinForm.Id,
            SessionId = SessionQuestionnaire.Id,
            IsSelf = true
        };
        _ = await FormsRepo.DeleteValuesFieldsByGroupSessionQuestionnaireByRowNum(req);
        await ReloadSession();
    }

    async Task ReloadSession()
    {
        if (SessionQuestionnaire is null)
        {
            SnackbarRepo.Add("SessionQuestionnaire is null. error {E4AE03AE-85A8-4D99-8C66-2FB46B84C67E}", Severity.Error, c => c.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
            return;
        }
        IsBusyProgress = true;
        FormSessionQuestionnaireResponseModel rest = string.IsNullOrWhiteSpace(SessionQuestionnaire.SessionToken)
        ? await FormsRepo.GetSessionQuestionnaire(SessionQuestionnaire.Id)
        : await FormsRepo.GetSessionQuestionnaire(SessionQuestionnaire.SessionToken);
        IsBusyProgress = false;

        SnackbarRepo.ShowMessagesResponse(rest.Messages);
        if (!rest.Success())
        {
            SnackbarRepo.Add($"Ошибка {{F046D8EB-DAA2-46AB-9C81-A43D5827BDA6}} Action: {rest.Message()}", Severity.Error, conf => conf.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
            return;
        }

        if (rest.SessionQuestionnaire is not null)
            SessionQuestionnaire.Reload(rest.SessionQuestionnaire);

        _table_kit_ref?.Update();

        StateHasChanged();
    }
}