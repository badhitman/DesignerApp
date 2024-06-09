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
    /// <inheritdoc/>
    [Inject]
    protected ISnackbar SnackbarRepo { get; set; } = default!;

    /// <inheritdoc/>
    [Inject]
    protected IFormsService FormsRepo { get; set; } = default!;

    /// <summary>
    /// Номер строки таблицы от 1 и больше.
    /// Если 0 (по умолчанию) => обрабатывается как [не таблица], а обычная форма
    /// </summary>
    [Parameter]
    public uint GroupByRowNum { get; set; }

    /// <inheritdoc/>
    [CascadingParameter, EditorRequired]
    public required ConstructorFormModelDB Form { get; set; }

    /// <inheritdoc/>
    [CascadingParameter]
    public ConstructorFormSessionModelDB? SessionQuestionnaire { get; set; }

    /// <inheritdoc/>
    [CascadingParameter]
    public ConstructorFormQuestionnairePageModelDB? QuestionnairePage { get; set; }

    /// <inheritdoc/>
    [CascadingParameter]
    public bool? InUse { get; set; }

    /// <inheritdoc/>
    [CascadingParameter]
    public ConstructorFormQuestionnairePageJoinFormModelDB? PageJoinForm { get; set; }

    /// <inheritdoc/>
    [CascadingParameter]
    public IEnumerable<EntryAltDescriptionModel> CalculationsAsEntries { get; set; } = default!;

    /// <inheritdoc/>
    [Parameter]
    public List<FieldComponentBaseModel?>? FieldsReferring { get; set; }

    /// <inheritdoc/>
    public abstract string DomID { get; }

    /// <inheritdoc/>
    public static bool IsReadonly(ClaimsPrincipal clp, ConstructorFormSessionModelDB sq)
    {
        string? email = clp.Claims.FirstOrDefault(x => x.Type.Equals(ClaimTypes.Email, StringComparison.OrdinalIgnoreCase))?.Value;
        return !clp.Claims.Any(x => x.Type.Equals(ClaimTypes.Role, StringComparison.OrdinalIgnoreCase) && (x.Value.Equals("Admin", StringComparison.OrdinalIgnoreCase))) && sq.SessionStatus >= SessionsStatusesEnum.Sended && !sq.CreatorEmail.Equals(email, StringComparison.OrdinalIgnoreCase);
    }

    /// <inheritdoc/>
    public async Task SetValue(string? valAsString, string fieldName)
    {
        if (InUse != true)
            return;

        if (PageJoinForm is null)
        {
            SnackbarRepo.Add("PageJoinForm is null. error {3098E5B7-DEA6-4A9A-A2A0-3119194401ED}", Severity.Error, c => c.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
            return;
        }
        if (SessionQuestionnaire is null)
        {
            SnackbarRepo.Add("SessionQuestionnaire is null. error {C18CEBB7-C245-4E00-B9A2-CBB046DF590F}", Severity.Error, c => c.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
            return;
        }

        SetValueFieldSessionQuestionnaireModel req = new()
        {
            FieldValue = valAsString,
            GroupByRowNum = GroupByRowNum,
            JoinFormId = PageJoinForm.Id,
            NameField = fieldName,
            SessionId = SessionQuestionnaire.Id
        };
        IsBusyProgress = true;
        FormSessionQuestionnaireResponseModel rest = await FormsRepo.SetValueFieldSessionQuestionnaire(req);
        IsBusyProgress = false;

        if (!rest.Success())
        {
            SnackbarRepo.Add($"Ошибка {{1CA79BA7-295C-40AD-BCF3-F6143DCCF2BD}}: {rest.Message()}", Severity.Error, conf => conf.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
            return;
        }
        if (rest.SessionQuestionnaire is null)
        {
            SnackbarRepo.Add($"{nameof(rest.SessionQuestionnaire)} is null. error {{13B77737-55FA-42C6-9B82-BD35F3740825}}", Severity.Error, conf => conf.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);
            return;
        }

        SessionQuestionnaire.Reload(rest.SessionQuestionnaire);

        FieldsReferring?
            .Where(x => x?.DomID.Equals(DomID, StringComparison.Ordinal) != true)
            .ToList()
            .ForEach(x => x?.StateHasChanged());
    }
}