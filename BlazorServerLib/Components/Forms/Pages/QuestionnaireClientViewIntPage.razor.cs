////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

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

    SessionOfDocumentDataModelDB SessionQuestionnaire = default!;

    /// <inheritdoc/>
    protected IEnumerable<EntryAltDescriptionModel> Entries = default!;

    /// <inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
        Entries = DeclarationAbstraction.CommandsAsEntries<VirtualColumnCalculationAbstraction>();
        IsBusyProgress = true;
        TResponseModel<SessionOfDocumentDataModelDB> rest = await FormsRepo.GetSessionQuestionnaire(QuestionnaireId);
        IsBusyProgress = false;

        if (rest.Response is null)
            throw new Exception("rest.Content.SessionQuestionnaire is null. error 09DFC142-55DA-4616-AA14-EA1B810E9A7E");

        SessionQuestionnaire = rest.Response;
        if (SessionQuestionnaire.SessionValues is not null && SessionQuestionnaire.SessionValues.Count != 0)
            SessionQuestionnaire.SessionValues.ForEach(x => { x.Owner ??= SessionQuestionnaire; x.OwnerId = SessionQuestionnaire.Id; });
    }
}