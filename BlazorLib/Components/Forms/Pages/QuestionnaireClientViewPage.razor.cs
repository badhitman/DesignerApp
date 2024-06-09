using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using SharedLib;

namespace BlazorLib.Components.Forms.Pages;

/// <summary>
/// 
/// </summary>
public partial class QuestionnaireClientViewPage : BlazorBusyComponentBaseModel
{
    [Inject]
    protected ILogger<QuestionnaireClientViewPage> _logger { get; set; } = default!;

    [Inject]
    protected IJSRuntime _js_runtime { get; set; } = default!;

    [Inject]
    protected IFormsService _forms { get; set; } = default!;

    /// <inheritdoc/>
    [Parameter, EditorRequired]
    public Guid QuestionnaireGuid { get; set; } = default!;

    ConstructorFormSessionModelDB SessionQuestionnaire = default!;

    /// <inheritdoc/>
    protected IEnumerable<EntryAltDescriptionModel> Entries = [];

    /// <inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
        Entries = DeclarationAbstraction.CommandsAsEntries<VirtualColumnCalcAbstraction>();
        IsBusyProgress = true;
        FormSessionQuestionnaireResponseModel rest = await _forms.GetSessionQuestionnaire(QuestionnaireGuid.ToString());
        IsBusyProgress = false;

        if (rest.SessionQuestionnaire is null)
            throw new Exception($"rest.SessionQuestionnaire is null. error {{AB30D092-938E-460A-B5AB-7E3BEC6A642A}}");

        SessionQuestionnaire = rest.SessionQuestionnaire;
        if (SessionQuestionnaire.SessionValues?.Any() == true)
            SessionQuestionnaire.SessionValues.ForEach(x => { x.Owner ??= SessionQuestionnaire; x.OwnerId = SessionQuestionnaire.Id; });
    }
}