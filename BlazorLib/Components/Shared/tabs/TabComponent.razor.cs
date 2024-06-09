using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorLib.Components.Shared.tabs;

/// <summary>
/// Tab component
/// </summary>
public partial class TabComponent : ComponentBase, ITab
{

    /// <inheritdoc/>
    [CascadingParameter]
    public TabSetComponent? ContainerTabSet { get; set; }

    /// <inheritdoc/>
    [Parameter]
    public string? Title { get; set; }

    /// <inheritdoc/>
    [Parameter]
    public string? Tooltip { get; set; }

    /// <inheritdoc/>
    [Parameter]
    public bool IsDisabled { get; set; } = false;

    /// <inheritdoc/>
    [Parameter]
    public Action? OnClickHandle { get; set; }

    /// <inheritdoc/>
    [Parameter, EditorRequired]
    public required RenderFragment ChildContent { get; set; }

    private string? TitleCssClass =>
        ContainerTabSet?.ActiveTab == this ? "active" : IsDisabled ? "disabled" : null;

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        ContainerTabSet?.AddTab(this);
    }

    void ActivateTabHandler(MouseEventArgs args)
    {
        ActivateTab();
        if (OnClickHandle is not null)
            OnClickHandle();
    }

    /// <inheritdoc/>
    public void ActivateTab()
    {
        ContainerTabSet?.SetActiveTab(this);
    }
}