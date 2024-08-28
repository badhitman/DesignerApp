////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
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
    /// Tab
    /// </summary>
    [CascadingParameter, EditorRequired]
    public required TabOfDocumentSchemeConstructorModelDB Tab { get; set; }

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
    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        if (Session is not null)
        {
            _readed_tab_id = TabOfDocument.Id;

            IsBusyProgress = true;
            SessionValues = [.. await JournalRepo.ReadSessionTabValues(Tab.Id, Session.Id)];
            IsBusyProgress = false;
        }
    }

    /// <inheritdoc/>
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (Session is not null && _readed_tab_id != TabOfDocument.Id)
        {
            _readed_tab_id = TabOfDocument.Id;

            IsBusyProgress = true;
            SessionValues = [.. await JournalRepo.ReadSessionTabValues(Tab.Id, Session.Id)];
            IsBusyProgress = false;
            StateHasChanged();
        }
    }
}