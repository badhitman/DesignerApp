////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

namespace SharedLib;

/// <summary>
/// Производство кода C#
/// </summary>
public interface IManufactureService
{
    /// <summary>
    /// Прочитать конфигурацию генератора кода
    /// </summary>
    public Task<TResponseModel<ManageManufactureModelDB>> ReadManufactureConfig(int projectId, string? userId = null);
}
