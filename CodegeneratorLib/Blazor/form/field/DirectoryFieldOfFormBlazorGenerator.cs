////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov
////////////////////////////////////////////////

using CodegeneratorLib;
using HtmlGenerator.html5;

namespace HtmlGenerator.blazor;

/// <summary>
/// Simple Field
/// </summary>
public class DirectoryFieldOfFormBlazorGenerator : safe_base_dom_root
{
    /// <summary>
    /// Field
    /// </summary>
    public required FieldAkaDirectoryFitModel Field { get; set; }

    /// <summary>
    /// Форма
    /// </summary>
    public required EntrySchemaTypeModel Form { get; set; }


}
