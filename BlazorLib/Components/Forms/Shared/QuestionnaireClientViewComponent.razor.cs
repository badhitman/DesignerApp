using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using MudBlazor;
using SharedLib;

namespace BlazorLib.Components.Forms.Shared;

public partial class QuestionnaireClientViewComponent : ComponentBase
{
    [Inject]
    protected ILogger<QuestionnaireClientViewComponent> _logger { get; set; } = default!;

    [Inject]
    protected ISnackbar _snackbar { get; set; } = default!;

    [CascadingParameter, EditorRequired]
    public ConstructorFormSessionModelDB SessionQuestionnaire { get; set; } = default!;

    [CascadingParameter, EditorRequired]
    public bool InUse { get; set; } = default!;

    protected MarkupString ms => (MarkupString)(!string.IsNullOrWhiteSpace(SessionQuestionnaire.Description) ? SessionQuestionnaire.Description : SessionQuestionnaire.Owner!.Description!);

    public MudDynamicTabs? DynamicTabs;
    public int QuestionnaireIndex;

    protected string AddIconStyleInUse => InUse ? "display:none;" : "";
}