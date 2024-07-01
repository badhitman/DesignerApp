////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using System.ComponentModel.DataAnnotations;

namespace SharedLib.Models;

/// <summary>
/// Вещественная модель (базовая с описанием)
/// </summary>
public class RealTypeModel : EntryDescriptionModel
{
    /// <summary>
    /// Системное имя (имя типа/класса C#)
    /// </summary>
    [RegularExpression(GlobalStaticConstants.NAME_SPACE_TEMPLATE, ErrorMessage = GlobalStaticConstants.NAME_SPACE_TEMPLATE_MESSAGE)]
    public required string SystemName { get; set; }
}