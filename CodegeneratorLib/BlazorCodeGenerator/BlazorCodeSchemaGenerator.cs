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
        DomElements = BlazorHtmlGenerator.FormEditPage(form_type_entry);
        Methods.Clear();
        return this;
    }
}