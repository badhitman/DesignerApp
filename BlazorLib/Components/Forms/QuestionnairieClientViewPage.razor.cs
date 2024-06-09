using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using SharedLib;

namespace BlazorLib.Components.Forms;

/// <summary>
/// 
/// </summary>
public partial class QuestionnairieClientViewPage : BlazorBusyComponentBaseModel
{
    [Inject]
    protected ILogger<QuestionnairieClientViewPage> _logger { get; set; } = default!;

    [Inject]
    protected IJSRuntime _js_runtime { get; set; } = default!;

    [Inject]
    protected IFormsService _forms { get; set; } = default!;

    /// <inheritdoc/>
    [Parameter, EditorRequired]
    public Guid QuestionnaireGuid { get; set; } = default!;

    ConstructorFormSessionModelDB SessionQuestionnairie = default!;

    /// <inheritdoc/>
    protected IEnumerable<EntryAltDescriptionModel> Entries = [];

    /// <inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
        Entries = DeclarationAbstraction.CommandsAsEntries<VirtualColumnCalcAbstraction>();
        IsBusyProgress = true;
        FormSessionQuestionnairieResponseModel rest = await _forms.GetSessionQuestionnairie(QuestionnaireGuid.ToString());
        IsBusyProgress = false;

        if (rest.SessionQuestionnairie is null)
            throw new Exception($"rest.SessionQuestionnaire is null. error {{AB30D092-938E-460A-B5AB-7E3BEC6A642A}}");

        SessionQuestionnairie = rest.SessionQuestionnairie;
        if (SessionQuestionnairie.SessionValues?.Any() == true)
            SessionQuestionnairie.SessionValues.ForEach(x => { x.Owner ??= SessionQuestionnairie; x.OwnerId = SessionQuestionnairie.Id; });
    }
}