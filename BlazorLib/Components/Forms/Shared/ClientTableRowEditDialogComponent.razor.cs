using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using MudBlazor;
using SharedLib;

namespace BlazorLib.Components.Forms.Shared;

public partial class ClientTableRowEditDialogComponent : BlazorBusyComponentBaseModel
{
    [Inject]
    protected ILogger<ClientTableRowEditDialogComponent> _logger { get; set; } = default!;

    [Inject]
    protected IJSRuntime JsRuntimeRepo { get; set; } = default!;

    [Inject]
    protected ISnackbar SnackbarRepo { get; set; } = default!;

    [CascadingParameter]
    MudDialogInstance MudDialog { get; set; } = default!;

    [Parameter, EditorRequired]
    public uint RowNum { get; set; }

    [Parameter]
    public ConstructorFormSessionModelDB SessionQuestionnaire { get; set; } = default!;

    [Parameter]
    public ConstructorFormQuestionnairePageModelDB QuestionnairePage { get; set; } = default!;

    [Parameter, EditorRequired]
    public ConstructorFormQuestionnairePageJoinFormModelDB PageJoinForm { get; set; } = default!;

    protected List<ConstructorFormSessionValueModelDB> RowValuesSet = [];

    protected IEnumerable<EntryAltDescriptionModel> Entries = Enumerable.Empty<EntryAltDescriptionModel>();

    protected void HoldOfBusyAction(bool is_hold)
    {
        IsBusyProgress = is_hold;
        // StateHasChanged();
    }

    protected void Close() => InvokeAsync(async () =>
    {
        while (IsBusyProgress)
            await Task.Delay(100);

        MudDialog.Close(DialogResult.Ok(RowValuesSet));
    });

    protected override void OnInitialized()
    {
        Entries = DeclarationAbstraction.CommandsAsEntries<VirtualColumnCalculationAbstraction>();
        base.OnInitialized();
    }
}