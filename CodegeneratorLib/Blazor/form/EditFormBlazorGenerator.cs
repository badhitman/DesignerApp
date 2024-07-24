////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov
////////////////////////////////////////////////

using CodegeneratorLib;
using HtmlGenerator.bootstrap;

namespace HtmlGenerator.blazor;

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
        CardBody = [.. Form.Form.AllFields.Select(x => new FieldOfFormBlazorGenerator() { Field = x, Form = Form })];
        return base.GetHTML(deep);
    }
}