////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov
////////////////////////////////////////////////

using CodegeneratorLib;
using HtmlGenerator.html5;
using HtmlGenerator.html5.areas;
using HtmlGenerator.html5.textual;

namespace HtmlGenerator.bootstrap;

/// <summary>
/// Класс Web/DOM 
/// </summary>
public class EditDocumentBlazorGenerator : safe_base_dom_root
{
    /// <inheritdoc/>
    public EditDocumentBlazorGenerator()
    {
        AddCSS("card");
        Childs = [new div() { }.AddCSS("card-body").AddDomNode(new p("This is some text within a card body."))];
    }

    /// <summary>
    /// Документ
    /// </summary>
    public required EntryDocumentTypeModel Document { get; set; }

    /// <summary>
    /// div
    /// </summary>
    public override string tag_custom_name => "div";

    /// <inheritdoc/>
    public override string GetHTML(int deep = 0)
    {
        if (Childs is null)
            Childs = [];
        else
            Childs.Clear();

        Childs = [new MudTabsProvider() { TabsPanels = [.. Document.Document.Tabs.Select(x => new MudTabPanelProvider() { Text = x.Name })] }];

        return base.GetHTML(deep);
    }
}