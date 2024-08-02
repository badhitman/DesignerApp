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
    TabOfDocumentSchemeConstructorModelDB? _tabOfDocument;
    /// <summary>
    /// TabOfDocument
    /// </summary>
    public TabOfDocumentSchemeConstructorModelDB TabOfDocument => Document.Tabs!.First(x => x.SortIndex == TabMetadata.SortIndex);

    /// <summary>
    /// PK строки БД
    /// </summary>
    [CascadingParameter]
    public SessionOfDocumentDataModelDB? Session { get; set; }

    /// <summary>
    /// Document
    /// </summary>
    [CascadingParameter, EditorRequired]
    public required DocumentSchemeConstructorModelDB Document { get; set; }

    /// <summary>
    /// Данные/значения текущей сессии для выбранной вкладки
    /// </summary>
    protected ValueDataForSessionOfDocumentModelDB[]? SessionValues { get; private set; }

    /// <inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        if (Session is null || TabOfDocument.JoinsForms?.Count != TabMetadata.Forms.Length)
            return;

        IsBusyProgress = true;
        SessionValues = await JournalRepo.ReadSessionTabValues(TabOfDocument.Id, Session.Id);
        IsBusyProgress = false;
    }
}