﻿////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov
////////////////////////////////////////////////

using HtmlGenerator.html5.areas;
using HtmlGenerator.html5.forms;
using HtmlGenerator.html5;
using HtmlGenerator.mud;
using SharedLib;
using HtmlGenerator.bootstrap;

namespace CodegeneratorLib;

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
        {
            switch (ff.TypeField)
            {
                case TypesFieldsFormsEnum.Bool:
                    Childs.Add(new CheckboxInputBootstrap(Field.Name, $"{Field.SystemName}-{Form.Form.SystemName}-{Form.Tab.SystemName}-{Form.Document.SystemName}"));
                    break;
                case TypesFieldsFormsEnum.Int:
                    Childs.Add(new MudNumericFieldProvider() { IsDouble = false });
                    break;
                case TypesFieldsFormsEnum.Double:
                    Childs.Add(new MudNumericFieldProvider() { IsDouble = true });
                    break;
                case TypesFieldsFormsEnum.Time or TypesFieldsFormsEnum.Text or TypesFieldsFormsEnum.Date or TypesFieldsFormsEnum.DateTime or TypesFieldsFormsEnum.Password:
                    Childs.Add(new MudTextFieldProvider() { Label = ff.Name, DescriptorType = GetDescriptorType(ff.TypeField), InputType = GetType(ff.TypeField) });
                    break;
                default: throw new Exception();
            }
        }
        else if (Field is FieldAkaDirectoryFitModel fd)
            Childs.Add(new MudSelectProvider() { Label = Field.Name, Hint = Field.Hint, TypeNameEnum = fd.SystemName, Options = fd.Items.Select(x => (x.Id.ToString(), x.Name)).ToArray() });

        return base.GetHTML(deep);
    }


    static string GetType(TypesFieldsFormsEnum t) => t switch
    {
        TypesFieldsFormsEnum.Time => "InputType.Time",
        TypesFieldsFormsEnum.Date => "InputType.Date",
        TypesFieldsFormsEnum.DateTime => "InputType.DateTimeLocal",
        TypesFieldsFormsEnum.Password => "InputType.Password",
        TypesFieldsFormsEnum.Text => "InputType.Text",
        _ => throw new Exception()
    };

    static string GetDescriptorType(TypesFieldsFormsEnum t) => t switch
    {
        TypesFieldsFormsEnum.Time => "TimeOnly",
        TypesFieldsFormsEnum.Date => "DateOnly",
        TypesFieldsFormsEnum.DateTime => "DateTime",
        TypesFieldsFormsEnum.Password or TypesFieldsFormsEnum.Text => "string",
        _ => throw new Exception()
    };
}