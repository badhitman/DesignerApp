////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components;
using SharedLib;

namespace BlazorWebLib.Components.Documents.Forms;

/// <summary>
/// TableFormOfTabConstructorComponent
/// </summary>
public partial class TableFormOfTabConstructorComponent : FormBaseCore
{
    /// <inheritdoc/>
    [CascadingParameter, EditorRequired]
    public required List<ValueDataForSessionOfDocumentModelDB> SessionValues { get; set; }

    /// <summary>
    /// Form
    /// </summary>
    [CascadingParameter, EditorRequired]
    public required FormConstructorModelDB Form { get; set; }

    /// <summary>
    /// Таблица редактируется через свои собственные посадочные стрраницы формы
    /// </summary>
    public override bool IsEdited => false;
}