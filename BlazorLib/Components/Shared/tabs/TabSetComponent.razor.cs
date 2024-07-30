using Microsoft.AspNetCore.Components;

namespace BlazorLib.Components.Shared.tabs;

/// <summary>
/// TabSet
/// </summary>
public partial class TabSetComponent : ComponentBase
{
    [Inject]
    NavigationManager NavigationManager { get; set; } = default!;


    /// <inheritdoc/>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <inheritdoc/>
    public ITab? ActiveTab { get; private set; }

    /// <inheritdoc/>
    public List<ITab> Tabs { get; private set; } = [];

    string? _selectedTabName = null;

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        _selectedTabName = NavigationManager.GetTabNameFromUrl();
        if (!string.IsNullOrWhiteSpace(_selectedTabName) && Tabs.Any(x => x.SystemName.Equals(_selectedTabName, StringComparison.OrdinalIgnoreCase)))
            SetActiveTab(Tabs.First(x => x.SystemName.Equals(_selectedTabName, StringComparison.OrdinalIgnoreCase)));
        base.OnInitialized();
    }

    /// <inheritdoc/>
    public void AddTab(ITab tab)
    {
        if (!Tabs.Any(x => x.SystemName.Equals(tab.SystemName, StringComparison.OrdinalIgnoreCase)))
            Tabs.Add(tab);

        if (ActiveTab is null || (tab.SystemName.Equals(_selectedTabName, StringComparison.OrdinalIgnoreCase) && !ActiveTab.SystemName.Equals(tab.SystemName, StringComparison.OrdinalIgnoreCase)))
            SetActiveTab(tab);
    }

    /// <inheritdoc/>
    public void SetActiveTab(ITab tab)
    {
        if (!Tabs.Any(x => x.SystemName.Equals(tab.SystemName, StringComparison.OrdinalIgnoreCase)))
            Tabs.Add(tab);

        ActiveTab = tab;
        StateHasChanged();
    }
}