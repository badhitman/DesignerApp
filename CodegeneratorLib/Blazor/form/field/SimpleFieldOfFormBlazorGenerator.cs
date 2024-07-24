////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov
////////////////////////////////////////////////

using CodegeneratorLib;
using HtmlGenerator.html5;
using HtmlGenerator.html5.areas;
using HtmlGenerator.html5.forms;
using HtmlGenerator.mud;
using SharedLib;

namespace HtmlGenerator.blazor;

/// <summary>
/// Simple Field
/// </summary>
public class SimpleFieldOfFormBlazorGenerator : safe_base_dom_root
{
    /// <summary>
    /// Field
    /// </summary>
    public required FieldFitModel Field { get; set; }

    /// <summary>
    /// Форма
    /// </summary>
    public required EntrySchemaTypeModel Form { get; set; }

    div GetCheckBox()
    {
        string dom_id = $"{Field.SystemName}-{Form.Form.SystemName}-{Form.Tab.SystemName}-{Form.Document.SystemName}";
        return (div)new div() { }
        .AddDomNode(new input() { Id_DOM = dom_id, })
        .AddDomNode(new label(Field.Name, dom_id));
    }

    /// <inheritdoc/>
    public override string GetHTML(int deep = 0)
    {
        if (Childs is null)
            Childs = [];
        else
            Childs.Clear();

        switch (Field.TypeField)
        {
            case TypesFieldsFormsEnum.Bool:
                Childs.Add(GetCheckBox());
                break;
            case TypesFieldsFormsEnum.Double:
                Childs.Add(new MudNumericFieldProvider() { IsDouble = true });
                break;
            case TypesFieldsFormsEnum.Int:
                Childs.Add(new MudNumericFieldProvider() { IsDouble = false });
                break;
            case TypesFieldsFormsEnum.Time or TypesFieldsFormsEnum.Text or TypesFieldsFormsEnum.Date or TypesFieldsFormsEnum.DateTime:

                break;
            case TypesFieldsFormsEnum.Password:

                break;
            default:
                throw new Exception();
        }

        //TextInputBootstrap res = new(Field.Name, $"{Field.SystemName}-{Form.Form.SystemName}-{Form.Tab.SystemName}-{Form.Document.SystemName}");
        //res.AddCSS(Field.Css);
        //res.Hint = Field.Hint;
        //AddDomNode(res);

        return base.GetHTML(deep);
    }
}
