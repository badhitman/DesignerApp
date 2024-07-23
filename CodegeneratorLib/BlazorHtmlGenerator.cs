////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov
////////////////////////////////////////////////

using CodegeneratorLib;
using HtmlGenerator.bootstrap;
using HtmlGenerator.html5;

namespace HtmlGenerator;

/// <summary>
/// Примеры использования (Blazor)
/// </summary>
public static class BlazorHtmlGenerator
{
    /// <summary>
    /// DocumentEditPage
    /// </summary>
    public static List<safe_base_dom_root> DocumentEditPage(EntryDocumentTypeModel doc_obj)
    {
        List<safe_base_dom_root> res = [];
        res.Add(new EditDocumentBlazorGenerator() { Document = doc_obj });
        return res;
    }

    /// <summary>
    /// FormEditPage
    /// </summary>
    public static List<safe_base_dom_root> FormEditPage(EntrySchemaTypeModel form_type_entry)
    {
        List<safe_base_dom_root> res = [];
        res.Add(new EditFormBlazorGenerator() { Form = form_type_entry });
        return res;
    }
}