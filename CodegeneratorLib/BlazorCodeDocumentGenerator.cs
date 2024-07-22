////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov
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
    public BlazorCodeGenerator Set(EntryDocumentTypeModel doc_obj)
    {
        Methods.Clear();
        DomElements.Clear();

        return this;
    }
}