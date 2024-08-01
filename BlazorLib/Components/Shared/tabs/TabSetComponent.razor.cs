using Microsoft.AspNetCore.Components;
using SharedLib;

namespace BlazorLib.Components.Shared.tabs;

/// <summary>
/// TabSet
/// </summary>
public partial class TabSetComponent : ComponentBase
{
    [Inject]
    NavigationManager NavigationRepo { get; set; } = default!;


    /// <inheritdoc/>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// IsSilent
    /// </summary>
    /// <inheritdoc/>
    [Parameter]
    public bool IsSilent { get; set; }


    /// <inheritdoc/>
    public ITab? ActiveTab { get; private set; }

    /// <inheritdoc/>
    public List<ITab> Tabs { get; private set; } = [];

    string? _selectedTabName = null;

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        _selectedTabName = NavigationRepo.GetTabNameFromUrl();
        if (!string.IsNullOrWhiteSpace(_selectedTabName) && Tabs.Any(x => x.SystemName.Equals(_selectedTabName, StringComparison.OrdinalIgnoreCase)))
            SetActiveTab(Tabs.First(x => x.SystemName.Equals(_selectedTabName, StringComparison.OrdinalIgnoreCase)), true);
        base.OnInitialized();
    }

    /// <inheritdoc/>
    public void AddTab(ITab tab)
    {
        if (!Tabs.Any(x => x.SystemName.Equals(tab.SystemName, StringComparison.OrdinalIgnoreCase)))
            Tabs.Add(tab);

        if (ActiveTab is null || (tab.SystemName.Equals(_selectedTabName, StringComparison.OrdinalIgnoreCase) && !ActiveTab.SystemName.Equals(tab.SystemName, StringComparison.OrdinalIgnoreCase)))
            SetActiveTab(tab, true);
    }

    /// <inheritdoc/>
    public void SetActiveTab(ITab tab, bool isSilent)
    {
        if (!Tabs.Any(x => x.SystemName.Equals(tab.SystemName, StringComparison.OrdinalIgnoreCase)))
            Tabs.Add(tab);

        if (ActiveTab?.SystemName != tab.SystemName)
        {
            if (isSilent)
            {
                ActiveTab = tab;
                StateHasChanged();
            }
            else
            {
                _selectedTabName = NavigationRepo.GetTabNameFromUrl() ?? throw new Exception();

                Uri uriBuilder = new(NavigationRepo.Uri);
                uriBuilder = new(uriBuilder.AppendQueryParameter("TabName", _selectedTabName));

                NavigationRepo.NavigateTo(uriBuilder.ToString());
            }
        }

    }
}