////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

namespace SharedLib.Models;

/// <summary>
/// Базовая (лёгкая) вещественная модель
/// </summary>
public class BaseFitRealTypeModel : IdNameDescriptionSimpleModel
{
    /// <summary>
    /// Системное кодовое имя
    /// </summary>
    public required string SystemName { get; set; }
}