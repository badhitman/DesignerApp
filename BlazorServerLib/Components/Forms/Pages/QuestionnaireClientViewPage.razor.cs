using BlazorLib;
using Microsoft.AspNetCore.Components;
using SharedLib;

namespace BlazorWebLib.Components.Forms.Pages;

/// <summary>
/// 
/// </summary>
public partial class QuestionnaireClientViewPage : BlazorBusyComponentBaseModel
{
    /// <inheritdoc/>
    [Inject]
    protected IFormsService FormsRepo { get; set; } = default!;

    /// <inheritdoc/>
    [Parameter, EditorRequired]
    public Guid QuestionnaireGuid { get; set; } = default!;

    ConstructorFormSessionModelDB SessionQuestionnaire = default!;

    /// <inheritdoc/>
    protected IEnumerable<EntryAltDescriptionModel> Entries = [];

    /// <inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
        Entries = DeclarationAbstraction.CommandsAsEntries<VirtualColumnCalculationAbstraction>();
        IsBusyProgress = true;
        TResponseModel<ConstructorFormSessionModelDB> rest = await FormsRepo.GetSessionQuestionnaire(QuestionnaireGuid.ToString());
        IsBusyProgress = false;

        if (rest.Response is null)
            throw new Exception($"rest.SessionQuestionnaire is null. error {{AB30D092-938E-460A-B5AB-7E3BEC6A642A}}");

        SessionQuestionnaire = rest.Response;
        if (SessionQuestionnaire.SessionValues is not null && SessionQuestionnaire.SessionValues.Count != 0)
            SessionQuestionnaire.SessionValues.ForEach(x => { x.Owner ??= SessionQuestionnaire; x.OwnerId = SessionQuestionnaire.Id; });
    }
}