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
        ComponentNamespace = $"{Config.Namespace}.{Config.BlazorDirectoryPath}";
        ComponentName = doc_obj.BlazorComponentName();
        ComponentDescription = $"[doc: '{doc_obj.Document.Name}' `{doc_obj.Document.SystemName}`]";
        DomElements = [new EditDocumentBlazorGenerator() { Document = doc_obj }];
        Methods.Clear();
        return this;
    }
}