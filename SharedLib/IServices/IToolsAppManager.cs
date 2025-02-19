////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

namespace SharedLib;

/// <inheritdoc/>
public interface IToolsAppManager
{
    /// <inheritdoc/>
    public Task<ApiRestConfigModelDB[]> GetAllConfigurations();

    /// <inheritdoc/>
    public Task<ApiRestConfigModelDB> ReadConfiguration(int confId);

    /// <inheritdoc/>
    public Task<ResponseBaseModel> UpdateOrCreateConfig(ApiRestConfigModelDB req);

    /// <inheritdoc/>
    public Task<ResponseBaseModel> DeleteConfig(int confId);


    /// <inheritdoc/>
    public Task<SyncDirectoryModelDB[]> GetSyncDirectoriesForConfig(int confId);

    /// <inheritdoc/>
    public Task<ResponseBaseModel> UpdateOrCreateSyncDirectory(SyncDirectoryModelDB req);

    /// <inheritdoc/>
    public Task<ResponseBaseModel> DeleteSyncDirectory(int syncDirectoryId);


    /// <inheritdoc/>
    public Task<ExeCommandModelDB[]> GetExeCommandsForConfig(int confId);

    /// <inheritdoc/>
    public Task<ResponseBaseModel> UpdateOrCreateExeCommand(ExeCommandModelDB req);

    /// <inheritdoc/>
    public Task<ResponseBaseModel> DeleteExeCommand(int exeCommandId);
}