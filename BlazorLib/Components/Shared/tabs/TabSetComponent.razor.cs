using Microsoft.AspNetCore.Components;

namespace BlazorLib.Components.Shared.tabs;

/// <summary>
/// TabSet
/// </summary>
public partial class TabSetComponent : ComponentBase
{
    /// <inheritdoc/>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <inheritdoc/>
    [Parameter]
    public bool AsPills { get; set; }

    /// <inheritdoc/>
    [Parameter]
    public bool NavFill { get; set; }

    /// <inheritdoc/>
    public ITab? ActiveTab { get; private set; }

    /// <inheritdoc/>
    public void AddTab(ITab tab)
    {
        if (ActiveTab is null)
        {
            SetActiveTab(tab);
        }
    }

    /// <inheritdoc/>
    public void SetActiveTab(ITab tab)
    {
        if (ActiveTab != tab)
        {
            ActiveTab = tab;
            StateHasChanged();
        }
    }
}