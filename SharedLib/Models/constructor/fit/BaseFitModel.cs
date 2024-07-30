////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Базовая модель
/// </summary>
public class BaseFitModel
{
    /// <summary>
    /// SystemName
    /// </summary>
    public required string SystemName { get; set; }

    /// <summary>
    /// Имя объекта
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Описание/примечание для объекта
    /// </summary>
    public string? Description { get; set; }
}