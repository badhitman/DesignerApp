////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov
////////////////////////////////////////////////

namespace CodegeneratorLib;

/// <summary>
/// Blazor code-generator
/// </summary>
public partial class BlazorCodeGenerator
{
    /// <summary>
    /// Blazor document: code-generator
    /// </summary>
    public virtual BlazorCodeGenerator SetEditDocument(EntryDocumentTypeModel doc_obj, List<ParameterComponentModel>? parameters = null)
    {
        Parameters = parameters;
        ComponentDestination = $"BlazorLib.{Config.BlazorDirectoryPath}";
        ComponentName = doc_obj.BlazorComponentName();
        ComponentDescription = $"[doc: '{doc_obj.Document.Name}' `{doc_obj.Document.SystemName}`]";
        DomElements = [new EditDocumentBlazorGenerator() { Document = doc_obj }];
        Methods.Clear();
        return this;
    }
}