﻿////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov
////////////////////////////////////////////////

using HtmlGenerator.bootstrap;

namespace CodegeneratorLib;

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
        CardBody = [.. Form.Form.AllFields.Select(x => new WrapperFieldOfFormBlazorGenerator() { Field = x, Form = Form })];
        return base.GetHTML(deep);
    }
}