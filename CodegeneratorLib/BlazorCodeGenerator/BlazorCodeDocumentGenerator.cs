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
    /// Blazor document: code-generator
    /// </summary>
    public virtual BlazorCodeGenerator Set(EntryDocumentTypeModel doc_obj)
    {
        DomElements = BlazorHtmlGenerator.DocumentEditPage(doc_obj);
        Methods.Clear();
        return this;
    }
}