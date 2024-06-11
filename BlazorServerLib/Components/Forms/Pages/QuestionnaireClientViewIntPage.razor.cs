using BlazorLib;
using Microsoft.AspNetCore.Components;
using SharedLib;

namespace BlazorWebLib.Components.Forms.Pages;

/// <inheritdoc/>
public partial class QuestionnaireClientViewIntPage : BlazorBusyComponentBaseModel
{
    /// <inheritdoc/>
    [Inject]
    protected IFormsService FormsRepo { get; set; } = default!;

    /// <inheritdoc/>
    [Parameter, EditorRequired]
    public int QuestionnaireId { get; set; }

    ConstructorFormSessionModelDB SessionQuestionnaire = default!;

    /// <inheritdoc/>
    protected IEnumerable<EntryAltDescriptionModel> Entries = default!;

    /// <inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
        Entries = DeclarationAbstraction.CommandsAsEntries<VirtualColumnCalculationAbstraction>();
        IsBusyProgress = true;
        FormSessionQuestionnaireResponseModel rest = await FormsRepo.GetSessionQuestionnaire(QuestionnaireId);
        IsBusyProgress = false;

        if (rest.SessionQuestionnaire is null)
            throw new Exception($"rest.Content.SessionQuestionnaire is null. error {{AB30D092-938E-460A-B5AB-7E3BEC6A642A}}");

        SessionQuestionnaire = rest.SessionQuestionnaire;
        if (SessionQuestionnaire.SessionValues is not null && SessionQuestionnaire.SessionValues.Count != 0)
            SessionQuestionnaire.SessionValues.ForEach(x => { x.Owner ??= SessionQuestionnaire; x.OwnerId = SessionQuestionnaire.Id; });
    }
}