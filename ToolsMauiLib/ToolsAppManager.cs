////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;
using SharedLib;
using DbcLib;

namespace ToolsMauiLib;

/// <summary>
/// ToolsAppManager
/// </summary>
public class ToolsAppManager(IDbContextFactory<ToolsAppContext> toolsDbFactory) : IToolsAppManager
{
    /// <inheritdoc/>
    public async Task<ApiRestConfigModelDB[]> GetAllConfigurations()
    {
        using ToolsAppContext context = await toolsDbFactory.CreateDbContextAsync();
        return await context.Configurations.ToArrayAsync();
    }

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> DeleteConfig(int confId)
    {
        using ToolsAppContext context = await toolsDbFactory.CreateDbContextAsync();

        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> DeleteExeCommand(int exeCommandId)
    {
        using ToolsAppContext context = await toolsDbFactory.CreateDbContextAsync();

        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> DeleteSyncDirectory(int syncDirectoryId)
    {
        using ToolsAppContext context = await toolsDbFactory.CreateDbContextAsync();

        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public async Task<ExeCommandModelDB[]> GetExeCommandsForConfig(int confId)
    {
        using ToolsAppContext context = await toolsDbFactory.CreateDbContextAsync();

        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public async Task<SyncDirectoryModelDB[]> GetSyncDirectoriesForConfig(int confId)
    {
        using ToolsAppContext context = await toolsDbFactory.CreateDbContextAsync();

        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public async Task<ApiRestConfigModelDB> ReadConfiguration(int confId)
    {
        using ToolsAppContext context = await toolsDbFactory.CreateDbContextAsync();

        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> UpdateOrCreateConfig(ApiRestConfigModelDB req)
    {
        using ToolsAppContext context = await toolsDbFactory.CreateDbContextAsync();

        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> UpdateOrCreateExeCommand(ExeCommandModelDB req)
    {
        using ToolsAppContext context = await toolsDbFactory.CreateDbContextAsync();

        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> UpdateOrCreateSyncDirectory(SyncDirectoryModelDB req)
    {
        using ToolsAppContext context = await toolsDbFactory.CreateDbContextAsync();

        throw new NotImplementedException();
    }
}