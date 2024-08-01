using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using SharedLib;

namespace BlazorLib.Components.Shared.tabs;

/// <summary>
/// Tab component
/// </summary>
public partial class TabComponent : ComponentBase, ITab
{
    [Inject]
    NavigationManager NavigationManager { get; set; } = default!;


    /// <inheritdoc/>
    [CascadingParameter, EditorRequired]
    public required TabSetComponent ContainerTabSet { get; set; }

    /// <inheritdoc/>
    [Parameter, EditorRequired]
    public required string SystemName { get; set; }

    /// <inheritdoc/>
    [Parameter, EditorRequired]
    public required string Title { get; set; }

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
        if (OnClickHandle is not null)
            OnClickHandle();

        ActivateTab();
    }

    /// <inheritdoc/>
    public void ActivateTab()
    {
        ContainerTabSet.SetActiveTab(this);
        if (!ContainerTabSet.IsSilent)
        {
            Uri _u = new(NavigationManager.Uri);
            _u.AppendQueryParameter("TabName", SystemName);
            NavigationManager.NavigateTo($"{_u}");
        }
    }
}