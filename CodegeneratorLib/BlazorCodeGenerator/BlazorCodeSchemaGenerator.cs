////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov
////////////////////////////////////////////////

using HtmlGenerator;

namespace CodegeneratorLib;

/// <summary>
/// Blazor code-generator
/// </summary>
public partial class BlazorCodeGenerator
{
    /// <summary>
    /// Blazor form: code-generator
    /// </summary>
    public virtual BlazorCodeGenerator SetForm(EntrySchemaTypeModel form_type_entry, List<ParameterComponentModel>? parameters = null)
    {
        Parameters = parameters;
        ComponentDestination = $"BlazorLib.{Config.BlazorDirectoryPath}.{EntrySchemaTypeModel.FormsSegmentName}";
        ComponentName = form_type_entry.BlazorComponentName();
        ComponentDescription = $"[doc: '{form_type_entry.Document.Name}' `{form_type_entry.Document.SystemName}`] [tab: '{form_type_entry.Tab.Name}' `{form_type_entry.Tab.SystemName}`] [form: '{form_type_entry.Form.Name}' `{form_type_entry.Form.SystemName}`]";
        DomElements = [new EditFormBlazorGenerator() { Form = form_type_entry }];
        //
        return this;
    }
}