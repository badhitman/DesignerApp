////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov
////////////////////////////////////////////////

using CodegeneratorLib;
using HtmlGenerator.bootstrap;
using HtmlGenerator.html5;

namespace HtmlGenerator.blazor;

/// <summary>
/// Поле формы
/// </summary>
public class FieldOfFormBlazorGenerator : safe_base_dom_root
{
    /// <summary>
    /// Поле формы
    /// </summary>
    public required BaseRequiredFormFitModel Field { get; set; }

    /// <summary>
    /// Форма
    /// </summary>
    public required EntrySchemaTypeModel Form { get; set; }

    /// <inheritdoc/>
    public override string? tag_custom_name => "div";

    /// <inheritdoc/>
    public override string GetHTML(int deep = 0)
    {
        if (Childs is null)
            Childs = [];
        else
            Childs.Clear();

        TextInputBootstrap res = new(Field.Name, $"{Field.SystemName}-{Form.Form.SystemName}-{Form.Tab.SystemName}-{Form.Document.SystemName}");
        res.AddCSS(Field.Css);

        AddDomNode(res);

        return base.GetHTML(deep);
    }
}