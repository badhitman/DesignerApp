////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
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

    /// <summary>
    /// Обновить конфигурацию генератора кода
    /// </summary>
    public Task<ResponseBaseModel> UpdateManufactureConfig(ManageManufactureModelDB manufacture);

    /// <summary>
    /// Установить системное имя сущности.
    /// </summary>
    /// <remarks>
    /// Если установить null (или пустую строку), тогда значение удаляется
    /// </remarks>
    public Task<ResponseBaseModel> SetOrDeleteSystemName(UpdateSystemNameModel request);

    /// <summary>
    /// Получить системные имена для генератора кода
    /// </summary>
    public Task<List<SystemNameEntryModel>> GetSystemNames(int manufactureId);

    /// <summary>
    /// Create snapshot
    /// </summary>
    public Task CreateSnapshot(StructureProjectModel dump, int projectId, string name);
}