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
    protected List<ValueDataForSessionOfDocumentModelDB>? SessionValues { get; private set; }

    int _readed_tab_id;
    
    /// <inheritdoc/>
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {

        }
        else
        {
            if (Session is null || TabOfDocument.JoinsForms?.Count != TabMetadata.Forms.Length)
                return;

            if (_readed_tab_id != TabOfDocument.Id)
            {
                _readed_tab_id = TabOfDocument.Id;

                IsBusyProgress = true;
                SessionValues = [..await JournalRepo.ReadSessionTabValues(TabOfDocument.Id, Session.Id)];
                IsBusyProgress = false;
            }
        }
    }
}