using BlazorLib;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using SharedLib;

namespace BlazorWebLib.Components.Forms.Shared;

/// <summary>
/// Client table row edit dialog
/// </summary>
public partial class ClientTableRowEditDialogComponent : BlazorBusyComponentBaseModel
{
    [CascadingParameter]
    MudDialogInstance MudDialog { get; set; } = default!;

    /// <inheritdoc/>
    [Parameter, EditorRequired]
    public uint RowNum { get; set; }

    /// <inheritdoc/>
    [Parameter]
    public ConstructorFormSessionModelDB SessionQuestionnaire { get; set; } = default!;

    /// <inheritdoc/>
    [Parameter]
    public ConstructorFormQuestionnairePageModelDB QuestionnairePage { get; set; } = default!;

    /// <inheritdoc/>
    [Parameter, EditorRequired]
    public ConstructorFormQuestionnairePageJoinFormModelDB PageJoinForm { get; set; } = default!;

    /// <inheritdoc/>
    protected List<ConstructorFormSessionValueModelDB> RowValuesSet = [];

    /// <inheritdoc/>
    protected IEnumerable<EntryAltDescriptionModel> Entries = [];

    /// <inheritdoc/>
    protected void Close() => InvokeAsync(async () =>
    {
        while (IsBusyProgress)
            await Task.Delay(100);

        MudDialog.Close(DialogResult.Ok(RowValuesSet));
    });

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        Entries = DeclarationAbstraction.CommandsAsEntries<VirtualColumnCalculationAbstraction>();
    }
}