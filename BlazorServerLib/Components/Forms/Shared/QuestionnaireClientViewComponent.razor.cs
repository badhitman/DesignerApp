////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components;
using MudBlazor;
using SharedLib;

namespace BlazorWebLib.Components.Forms.Shared;

/// <summary>
/// Questionnaire client view
/// </summary>
public partial class QuestionnaireClientViewComponent : ComponentBase
{
    [Inject]
    ISnackbar SnackbarRepo { get; set; } = default!;

    /// <summary>
    /// Session questionnaire
    /// </summary>
    [CascadingParameter, EditorRequired]
    public required ConstructorFormSessionModelDB SessionQuestionnaire { get; set; }

    /// <inheritdoc/>
    [CascadingParameter, EditorRequired]
    public bool InUse { get; set; } = default!;

    /// <summary>
    /// Информация
    /// </summary>
    protected MarkupString Information => (MarkupString)(!string.IsNullOrWhiteSpace(SessionQuestionnaire.Description) ? SessionQuestionnaire.Description : SessionQuestionnaire.Owner!.Description!);

    /// <summary>
    /// Dynamic tabs - ref
    /// </summary>
    public MudDynamicTabs? DynamicTabs_ref;

    /// <summary>
    /// Questionnaire index
    /// </summary>
    public int QuestionnaireIndex;

    /// <summary>
    /// В зависимости режима (InUse) стили, которые добавятся к кнопке добавления: Если документ используется для реального заполнения, то кнопка скрывается.
    /// </summary>
    protected string AddIconStyleInUse => InUse ? "display:none;" : "";
}