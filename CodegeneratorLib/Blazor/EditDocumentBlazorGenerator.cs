////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov
////////////////////////////////////////////////

using HtmlGenerator.bootstrap;
using HtmlGenerator.mud;
using SharedLib;

namespace CodegeneratorLib;

/// <summary>
/// Класс Web/DOM 
/// </summary>
public class EditDocumentBlazorGenerator : CardBootstrap
{
    /// <summary>
    /// Документ
    /// </summary>
    public required EntryDocumentTypeModel Document { get; set; }

    /// <inheritdoc/>
    public override string GetHTML(int deep = 0)
    {
        CardBody = [new MudTabsProvider() { TabsPanels = [.. Document.Document.Tabs.Select(ConvertTab)] }];
        return base.GetHTML(deep);
    }

    /// <summary>
    /// Convert Tab: TabFitModel to MudTabPanelProvider
    /// </summary>
    public MudTabPanelProvider ConvertTab(TabFitModel _tab)
    {
        MudTabPanelProvider res = new()
        {
            Text = _tab.Name,
            BodyElements = _tab.Forms.Select(x => new EditFormBlazorGenerator() { Form = new(x, _tab, Document.Document, "*", "*") })
        };
        return res;
    }
}