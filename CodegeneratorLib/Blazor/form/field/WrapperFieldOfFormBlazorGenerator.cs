////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov
////////////////////////////////////////////////

using CodegeneratorLib;
using HtmlGenerator.html5;

namespace HtmlGenerator.blazor;

/// <summary>
/// Поле формы (определение требуемой модели поля формы)
/// </summary>
public class WrapperFieldOfFormBlazorGenerator : safe_base_dom_root
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

        AddCSS(string.IsNullOrWhiteSpace(Field.Css) ? "col-12" : Field.Css);

        if (Field is FieldFitModel ff)
            Childs.Add(new SimpleFieldOfFormBlazorGenerator() { Field = ff, Form = Form });
        else if (Field is FieldAkaDirectoryFitModel fd)
            Childs.Add(new DirectoryFieldOfFormBlazorGenerator() { Field = fd, Form = Form });

        return base.GetHTML(deep);
    }
}