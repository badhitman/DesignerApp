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
public class EditFormBlazorGenerator : safe_base_dom_root
{
    /// <summary>
    /// Форма
    /// </summary>
    public required EntrySchemaTypeModel Form { get; set; }

    /// <inheritdoc/>
    /// <remarks>При вызове этого метода поле Childs очищается и заново заполняется</remarks>
    public override string GetHTML(int deep = 0)
    {
        if (Childs is null)
            Childs = [];
        else
            Childs.Clear();

        Childs = [new div() { }.AddCSS("card").AddDomNode(new div() { }.AddCSS("card-body").AddDomNode(new p("This is some text within a card body.")))];

        return base.GetHTML(deep);
    }
}