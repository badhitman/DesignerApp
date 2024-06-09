using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using SharedLib;

namespace BlazorLib.Components.Forms.Pages;

/// <summary>
/// 
/// </summary>
public partial class QuestionnairieClientViewIntPage : BlazorBusyComponentBaseModel
{
    /// <inheritdoc/>
    [Inject]
    protected ILogger<QuestionnairieClientViewPage> _logger { get; set; } = default!;

    /// <inheritdoc/>
    [Inject]
    protected IFormsService _forms { get; set; } = default!;

    /// <inheritdoc/>
    [Parameter, EditorRequired]
    public int QuestionnaireId { get; set; } = default!;

    ConstructorFormSessionModelDB SessionQuestionnairie = default!;

    /// <inheritdoc/>
    protected IEnumerable<EntryAltDescriptionModel> Entries = default!;

    /// <inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
        Entries = DeclarationAbstraction.CommandsAsEntries<VirtualColumnCalcAbstraction>();
        IsBusyProgress = true;
        FormSessionQuestionnairieResponseModel rest = await _forms.GetSessionQuestionnairie(QuestionnaireId);
        IsBusyProgress = false;

        if (rest.SessionQuestionnairie is null)
            throw new Exception($"rest.Content.SessionQuestionnaire is null. error {{AB30D092-938E-460A-B5AB-7E3BEC6A642A}}");

        SessionQuestionnairie = rest.SessionQuestionnairie;
        if (SessionQuestionnairie.SessionValues?.Any() == true)
            SessionQuestionnairie.SessionValues.ForEach(x => { x.Owner ??= SessionQuestionnairie; x.OwnerId = SessionQuestionnairie.Id; });
    }
}