////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;

namespace SharedLib;

/// <summary>
/// UniversalLayerModel
/// </summary>
[Index(nameof(NormalizedNameUpper)), Index(nameof(ContextName))]
[Index(nameof(SortIndex), nameof(ParentId), nameof(ContextName), IsUnique = true)]
public class UniversalLayerModel : UniversalBaseModel
{
    /// <summary>
    /// Имя контекста для разделения различных селекторов независимо друг от друга
    /// </summary>
    public string? ContextName { get; set; }

    /// <summary>
    /// ToUpper
    /// </summary>
    public string? NormalizedNameUpper { get; set; }
}