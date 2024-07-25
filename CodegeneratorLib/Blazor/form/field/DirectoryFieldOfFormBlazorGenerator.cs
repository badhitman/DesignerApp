////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov
////////////////////////////////////////////////

using CodegeneratorLib;
using HtmlGenerator.mud;

namespace CodegeneratorLib;

/// <summary>
/// Simple Field
/// </summary>
public class DirectoryFieldOfFormBlazorGenerator : MudSelectProvider
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
