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
    /// Parent tab
    /// </summary>
    [CascadingParameter, EditorRequired]
    public required TabOfDocumentConstructorComponent ParentTab { get; set; }

    /// <summary>
    /// Таб/Вкладка документа конструктора
    /// </summary>
    [CascadingParameter, EditorRequired]
    public required TabOfDocumentSchemeConstructorModelDB TabOfDocumentSchemeConstructor { get; set; }

    /// <summary>
    /// PK строки БД
    /// </summary>
    [CascadingParameter, EditorRequired]
    public required SessionOfDocumentDataModelDB Session { get; set; }

    /// <summary>
    /// Данные/значения текущей сессии для выбранной вкладки
    /// </summary>
    protected ValueDataForSessionOfDocumentModelDB[] SessionValues { get; private set; } = default!;

    /// <summary>
    /// Формы вкладки (сортированые)
    /// </summary>
    protected TabJoinDocumentSchemeConstructorModelDB[] FJoins => TabOfDocumentSchemeConstructor.JoinsForms!
        .Distinct()
        .OrderBy(x => x!.SortIndex)
        .ToArray()!;

    /// <inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
        IsBusyProgress = true;
        SessionValues = await JournalRepo.ReadSessionTabValues(TabOfDocumentSchemeConstructor.Id, Session.Id);
        IsBusyProgress = false;
    }
}