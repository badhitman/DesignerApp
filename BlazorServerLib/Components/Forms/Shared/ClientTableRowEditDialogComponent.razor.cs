////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using BlazorWebLib.Components.Forms.Pages;
using Microsoft.AspNetCore.Components;
using BlazorLib;
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
    public SessionOfDocumentDataModelDB SessionQuestionnaire { get; set; } = default!;

    /// <inheritdoc/>
    [Parameter]
    public TabOfDocumentSchemeConstructorModelDB QuestionnairePage { get; set; } = default!;

    /// <inheritdoc/>
    [Parameter, EditorRequired]
    public TabJoinDocumentSchemeConstructorModelDB PageJoinForm { get; set; } = default!;

    /// <inheritdoc/>
    [Parameter, EditorRequired]
    public required FormsPage ParentFormsPage { get; set; }


    /// <inheritdoc/>
    protected List<ValueDataForSessionOfDocumentModelDB> RowValuesSet = [];

    /// <inheritdoc/>
    protected IEnumerable<EntryAltDescriptionModel> Entries = [];

    /// <inheritdoc/>
    protected void Close() => InvokeAsync(async () =>
    {
        while (IsBusyProgress)
            await Task.Delay(100);

        MudDialog.Close(DialogResult.Ok(RowValuesSet));
    });
}