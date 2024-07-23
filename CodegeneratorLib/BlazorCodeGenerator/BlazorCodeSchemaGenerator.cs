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
    /// Blazor form: code-generator
    /// </summary>
    public BlazorCodeGenerator Set(EntrySchemaTypeModel form_type_entry)
    {
        Methods.Clear();
        DomElements.Clear();

        return this;
    }
}