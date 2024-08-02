////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Components;
using SharedLib;

namespace BlazorWebLib;

/// <summary>
/// TabOfDocumentComponent
/// </summary>
public abstract class TTabOfDocumenBaseComponent : DocumenBodyBaseComponent
{
    /// <summary>
    /// Tab Metadata
    /// </summary>
    [CascadingParameter, EditorRequired]
    public required TabFitModel TabMetadata { get; set; }

    /// <summary>
    /// ParentDocument
    /// </summary>
    [CascadingParameter, EditorRequired]
    public required DocumentEditBaseComponent ParentDocument { get; set; }

    /// <summary>
    /// TabChangeHandle
    /// </summary>
    [Parameter, EditorRequired]
    public required Action<TTabOfDocumenBaseComponent> TabChangeHandle { get; set; }


    /// <summary>
    /// Формы в табе
    /// </summary>
    public List<FormBaseModel> FormsStack { get; set; } = [];

    /// <inheritdoc/>
    public override bool IsEdited => FormsStack.Any(x => x.IsEdited);

    /// <inheritdoc/>
    protected override Task OnInitializedAsync()
    {
        ParentDocument.TabsComponents.Add(this);
        return base.OnInitializedAsync();
    }

    /// <summary>
    /// FormChangeEvent
    /// </summary>
    public void FormChangeEvent(FormBaseModel sender)
    {
        TabChangeHandle(this);
    }
}