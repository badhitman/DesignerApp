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
    public virtual BlazorCodeGenerator Set(EntrySchemaTypeModel form_type_entry)
    {
        ComponentName = form_type_entry.BlazorComponentName();
        ComponentDescription = $"[doc: '{form_type_entry.Document.Name}' `{form_type_entry.Document.SystemName}`] [tab: '{form_type_entry.Tab.Name}' `{form_type_entry.Tab.SystemName}`] [form: '{form_type_entry.Form.Name}' `{form_type_entry.Form.SystemName}`]";
        DomElements = BlazorHtmlGenerator.FormEditPage(form_type_entry);
        //
        return this;
    }
}