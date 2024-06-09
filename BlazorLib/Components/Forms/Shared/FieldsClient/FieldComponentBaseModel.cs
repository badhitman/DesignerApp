using Microsoft.AspNetCore.Components;
using System.Security.Claims;
using MudBlazor;
using SharedLib;

namespace BlazorLib.Components.Forms.Shared.FieldsClient;

/// <summary>
/// Field component base model
/// </summary>
public abstract class FieldComponentBaseModel : BlazorBusyComponentBaseModel
{
    [Inject]
    protected ISnackbar snackbarInject { get; set; } = default!;

    [Inject]
    protected IFormsService _forms { get; set; } = default!;

    /// <summary>
    /// Номер строки таблицы от 1 и больше.
    /// Если 0 (по умолчанию) => обрабатывается как [не таблица], а обычная форма
    /// </summary>
    [Parameter]
    public uint GroupByRowNum { get; set; }

    [CascadingParameter, EditorRequired]
    public ConstructorFormModelDB Form { get; set; } = default!;

    [CascadingParameter]
    public ConstructorFormSessionModelDB? SessionQuestionnairie { get; set; }

    [CascadingParameter]
    public ConstructorFormQuestionnairePageModelDB? QuestionnairePage { get; set; }

    [CascadingParameter]
    public bool? InUse { get; set; }

    [CascadingParameter]
    public ConstructorFormQuestionnairePageJoinFormModelDB? PageJoinForm { get; set; }

    [CascadingParameter]
    public IEnumerable<EntryAltDescriptionModel> CalculationsAsEntries { get; set; } = default!;

    [Parameter]
    public List<FieldComponentBaseModel?>? FieldsReferals { get; set; }

    public abstract string DomID { get; }

    public static bool IsReadonly(ClaimsPrincipal clp, ConstructorFormSessionModelDB sq)
    {
        string? email = clp.Claims.FirstOrDefault(x => x.Type.Equals(ClaimTypes.Email, StringComparison.OrdinalIgnoreCase))?.Value;
        return !clp.Claims.Any(x => x.Type.Equals(ClaimTypes.Role, StringComparison.OrdinalIgnoreCase) && (x.Value.Equals("Admin", StringComparison.OrdinalIgnoreCase))) && sq.SessionStatus >= SessionsStatusesEnum.Sended && !sq.CreatorEmail.Equals(email, StringComparison.OrdinalIgnoreCase);
    }

    public async Task SetValue(string? valAsString, string fieldName)
    {
        if (InUse != true)
            return;

        if (PageJoinForm is null)
        {
            snackbarInject.Add("PageJoinForm is null. error {3098E5B7-DEA6-4A9A-A2A0-3119194401ED}", Severity.Error, c => c.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
            return;
        }
        if (SessionQuestionnairie is null)
        {
            snackbarInject.Add("SessionQuestionnairie is null. error {C18CEBB7-C245-4E00-B9A2-CBB046DF590F}", Severity.Error, c => c.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
            return;
        }

        SetValueFieldSessionQuestionnairieModel req = new()
        {
            FieldValue = valAsString,
            GroupByRowNum = GroupByRowNum,
            JoinFormId = PageJoinForm.Id,
            NameField = fieldName,
            SessionId = SessionQuestionnairie.Id
        };
        IsBusyProgress = true;
        FormSessionQuestionnairieResponseModel rest = await _forms.SetValueFieldSessionQuestionnairie(req);
        IsBusyProgress = false;

        if (!rest.Success())
        {
            snackbarInject.Add($"Ошибка {{1CA79BA7-295C-40AD-BCF3-F6143DCCF2BD}}: {rest.Message()}", Severity.Error, conf => conf.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
            return;
        }
        if (rest.SessionQuestionnairie is null)
        {
            snackbarInject.Add($"{nameof(rest.SessionQuestionnairie)} is null. error {{13B77737-55FA-42C6-9B82-BD35F3740825}}", Severity.Error, conf => conf.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
            return;
        }

        SessionQuestionnairie.Reload(rest.SessionQuestionnairie);

        FieldsReferals?
            .Where(x => x?.DomID.Equals(DomID, StringComparison.Ordinal) != true)
            .ToList()
            .ForEach(x => x?.StateHasChanged());
    }
}