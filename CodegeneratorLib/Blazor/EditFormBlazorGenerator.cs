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
public class EditFormBlazorGenerator : CardBootstrap
{
    /// <summary>
    /// Форма
    /// </summary>
    public required EntrySchemaTypeModel Form { get; set; }

    /// <inheritdoc/>
    public override string GetHTML(int deep = 0)
    {
        CardBody = [new p("This is some text within a card body.")];
        return base.GetHTML(deep);
    }
}