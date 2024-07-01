////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

namespace SharedLib.Models;

/// <summary>
/// Манипуляция с элементами перечисления
/// </summary>
public class EnumItemActionRequestModel : IdNameDescriptionSimpleModel
{
    /// <summary>
    /// Идентификатор перечисления
    /// </summary>
    public int OwnerEnumId { get; set; }
}
