////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components;

namespace BlazorWebLib.Components;

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
    public required int TabId { get; set; }
}