using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using MudBlazor;
using SharedLib;

namespace BlazorLib.Components.Forms.Shared;

public partial class QuestionnairieClientViewComponent : ComponentBase
{
    [Inject]
    protected ILogger<QuestionnairieClientViewComponent> _logger { get; set; } = default!;

    [Inject]
    protected ISnackbar _snackbar { get; set; } = default!;

    [CascadingParameter, EditorRequired]
    public ConstructorFormSessionModelDB SessionQuestionnairie { get; set; } = default!;

    [CascadingParameter, EditorRequired]
    public bool InUse { get; set; } = default!;

    protected MarkupString ms => (MarkupString)(!string.IsNullOrWhiteSpace(SessionQuestionnairie.Description) ? SessionQuestionnairie.Description : SessionQuestionnairie.Owner!.Description!);

    public MudDynamicTabs? DynamicTabs;
    public int QuestionnaireIndex;

    protected string AddIconStyleInUse => InUse ? "display:none;" : "";
}