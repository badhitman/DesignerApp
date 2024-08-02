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
    /// Таб/Вкладка документа конструктора
    /// </summary>
    [CascadingParameter, EditorRequired]
    public required TabOfDocumentSchemeConstructorModelDB TabOfDocument { get; set; }

    /// <summary>
    /// PK строки БД
    /// </summary>
    [CascadingParameter]
    public SessionOfDocumentDataModelDB? Session { get; set; }


    /// <summary>
    /// Данные/значения текущей сессии для выбранной вкладки
    /// </summary>
    protected ValueDataForSessionOfDocumentModelDB[]? SessionValues { get; private set; }

    /// <summary>
    /// Формы вкладки (сортированые)
    /// </summary>
    protected TabJoinDocumentSchemeConstructorModelDB[] FJoins => TabOfDocument.JoinsForms!
        .Distinct()
        .OrderBy(x => x!.SortIndex)
        .ToArray()!;


    /// <inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
        if (Session is null)
            return;

        if (TabOfDocument.JoinsForms?.Count != TabMetadata.Forms.Length)
            throw new Exception();

        IsBusyProgress = true;
        SessionValues = await JournalRepo.ReadSessionTabValues(TabOfDocument.Id, Session.Id);
        IsBusyProgress = false;
    }
}