////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components;
using System.Security.Claims;
using MudBlazor;
using SharedLib;
using BlazorLib;
using BlazorLib.Components;

namespace BlazorWebLib.Components.Forms.Shared.FieldsClient;

/// <summary>
/// Базовая модель поля формы
/// </summary>
public abstract class FieldComponentBaseModel : BlazorBusyComponentBaseModel, IDomBaseComponent
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

    /// <summary>
    /// Форма
    /// </summary>
    [CascadingParameter, EditorRequired]
    public required FormConstructorModelDB Form { get; set; }

    /// <summary>
    /// Документ/сессия
    /// </summary>
    [CascadingParameter]
    public SessionOfDocumentDataModelDB? SessionQuestionnaire { get; set; }

    /// <summary>
    /// Страница/Таб документа
    /// </summary>
    [CascadingParameter]
    public TabOfDocumentSchemeConstructorModelDB? QuestionnairePage { get; set; }

    /// <summary>
    /// Признак того, что поле находится в состоянии реального использования, а не в конструкторе или режим demo
    /// </summary>
    public bool InUse => PageJoinForm is not null && SessionQuestionnaire is not null;

    /// <summary>
    /// Связь формы со страницей опроса/анкеты. В режиме DEMO тут NULL
    /// </summary>
    [CascadingParameter]
    public TabJoinDocumentSchemeConstructorModelDB? PageJoinForm { get; set; }

    /// <inheritdoc/>
    [CascadingParameter]
    public IEnumerable<EntryAltDescriptionModel> CalculationsAsEntries { get; set; } = default!;

    /// <summary>
    /// Доступ к перечню полей формы. Каждое поле формы добавляет себя к этому перечню при инициализации (в <c>OnInitialized()</c>) базового <cref name="FieldBaseClientComponent">компонента</cref>
    /// </summary>
    [Parameter]
    public List<FieldComponentBaseModel?>? FieldsReferring { get; set; }

    /// <inheritdoc/>
    public abstract string DomID { get; }

    /// <summary>
    /// Установить значение поля
    /// </summary>
    /// <param name="valAsString">Значение поля</param>
    /// <param name="fieldName">Имя поля</param>
    /// <exception cref="InvalidOperationException">В процессе установки значения не был возвращён объект сессии/документа, который необходим для обновления состояния <see cref="SessionQuestionnaire" />.</exception>
    /// <exception cref="NullReferenceException">Ошибка в случае если отсутствует <cref cref="PageJoinForm">связь</cref> (например: в режиме 'demo'). Полноценно инициированное поле формы должно так же иметь объект сессии <see cref="SessionQuestionnaire" />.</exception>
    public async Task SetValue(string? valAsString, string fieldName)
    {
        if (!InUse)
            throw new NullReferenceException("Форма не инициализирована.");

        SetValueFieldSessionQuestionnaireModel req = new()
        {
            FieldValue = valAsString,
            GroupByRowNum = GroupByRowNum,
            JoinFormId = PageJoinForm!.Id,
            NameField = fieldName,
            SessionId = SessionQuestionnaire!.Id
        };
        IsBusyProgress = true;
        TResponseModel<SessionOfDocumentDataModelDB> rest = await FormsRepo.SetValueFieldSessionQuestionnaire(req);
        IsBusyProgress = false;

        if (!rest.Success())
            SnackbarRepo.Add($"Ошибка B449F13A-7316-4BBB-96F0-DD2A7E6D6699: {rest.Message()}", Severity.Error, conf => conf.DuplicatesBehavior = SnackbarDuplicatesBehavior.Allow);

        if (rest.Response is null)
            throw new InvalidOperationException($"Данные сессии 'SessionQuestionnaire': IsNull");

        if (!rest.Success())
            return;

        SessionQuestionnaire.Reload(rest.Response);

        FieldsReferring?
            .Where(x => x?.DomID.Equals(DomID, StringComparison.Ordinal) != true)
            .ToList()
            .ForEach(x => x?.StateHasChanged());
    }
}