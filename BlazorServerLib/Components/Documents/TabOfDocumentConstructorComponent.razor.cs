////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components;
using SharedLib;

namespace BlazorWebLib.Components.Documents;

/// <summary>
/// TabOfDocumentConstructorComponent
/// </summary>
public partial class TabOfDocumentConstructorComponent : TTabOfDocumenBaseComponent
{
    /// <inheritdoc/>
    [Parameter, EditorRequired]
    public required int DocumentKey { get; set; }

    /// <inheritdoc/>
    [Parameter, EditorRequired]
    public required TabOfDocumentSchemeConstructorModelDB TabDB { get; set; }


    /// <summary>
    /// Session values
    /// </summary>
    protected ValueDataForSessionOfDocumentModelDB[] SessionValues { get; private set; } = default!;

    /// <summary>
    /// Tab joins
    /// </summary>
    protected TabJoinDocumentSchemeConstructorModelDB[] TabJoins => SessionValues
        .Select(x => x.TabJoinDocumentScheme)
        .Distinct()
        .OrderBy(x => x!.SortIndex)
        .ToArray()!;

    /// <inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
        IsBusyProgress = true;
        SessionValues = await JournalRepo.ReadSessionTabValues(TabDB.Id, DocumentKey);
        IsBusyProgress = false;
    }
}