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
    public async Task<ApiRestConfigModelDB> ReadConfiguration(int confId)
    {
        using ToolsAppContext context = await toolsDbFactory.CreateDbContextAsync();

        return await context
            .Configurations
            .Include(x => x.SyncDirectories)
            .Include(x => x.CommandsRemote)
            .FirstAsync(x => x.Id == confId);
    }

    /// <inheritdoc/>
    public async Task<TResponseModel<int>> UpdateOrCreateConfig(ApiRestConfigModelDB req)
    {
        req.Name = req.Name.Trim();

        req.HeaderName = req.HeaderName.Trim();
        req.TokenAccess = req.TokenAccess.Trim();
        req.AddressBaseUri = req.AddressBaseUri.Trim();

        TResponseModel<int> res = new();
        ValidateReportModel ch = GlobalTools.ValidateObject(req);
        if (!ch.IsValid)
        {
            res.Messages.InjectException(ch.ValidationResults);
            return res;
        }

        if (!req.AddressBaseUri.EndsWith('/'))
            req.AddressBaseUri = $"{req.AddressBaseUri}/";

        using ToolsAppContext context = await toolsDbFactory.CreateDbContextAsync();
        if (req.Id < 1)
        {
            req.SyncDirectories?.ForEach(x => x.Parent = req);
            req.CommandsRemote?.ForEach(x => x.Parent = req);

            req.Id = 0;
            await context.AddAsync(req);
            await context.SaveChangesAsync();
            res.AddSuccess("Токен успешно добавлен");
            res.Response = req.Id;
            return res;
        }

        res.Response = await context
             .Configurations
             .Where(x => x.Id == req.Id)
             .ExecuteUpdateAsync(set => set
             .SetProperty(p => p.AddressBaseUri, req.AddressBaseUri)
             .SetProperty(p => p.Name, req.Name)
             .SetProperty(p => p.TokenAccess, req.TokenAccess));

        res.AddInfo(res.Response == 0 ? "Изменений нет" : "Токен успешно обновлён");
        return res;
    }

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> DeleteConfig(int confId)
    {
        using ToolsAppContext context = await toolsDbFactory.CreateDbContextAsync();

        if (1 > await context
             .Configurations
             .Where(x => x.Id == confId)
             .ExecuteDeleteAsync())
            return ResponseBaseModel.CreateInfo("Объекта не существует");

        return ResponseBaseModel.CreateInfo("Токен успешно удалён");
    }

    /// <inheritdoc/>
    public async Task<ExeCommandModelDB[]> GetExeCommandsForConfig(int confId)
    {
        using ToolsAppContext context = await toolsDbFactory.CreateDbContextAsync();
        return await context.ExeCommands.Where(x => x.ParentId == confId).ToArrayAsync();
    }

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> DeleteExeCommand(int exeCommandId)
    {
        using ToolsAppContext context = await toolsDbFactory.CreateDbContextAsync();

        if (1 > await context
            .ExeCommands
            .Where(x => x.Id == exeCommandId)
            .ExecuteDeleteAsync())
            return ResponseBaseModel.CreateInfo("Команды не существует");

        return ResponseBaseModel.CreateInfo("Команда успешно удалена");
    }

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> UpdateOrCreateExeCommand(ExeCommandModelDB req)
    {
        using ToolsAppContext context = await toolsDbFactory.CreateDbContextAsync();

        if (req.Id < 1)
        {
            req.Parent = null;
            req.Id = 0;
            await context.AddAsync(req);
            await context.SaveChangesAsync();

            return ResponseBaseModel.CreateSuccess("Команда успешно добавлена");
        }

        if (1 > await context
             .ExeCommands
             .Where(x => x.Id == req.Id)
             .ExecuteUpdateAsync(set => set
             .SetProperty(p => p.Name, req.Name)
             .SetProperty(p => p.FileName, req.FileName)
             .SetProperty(p => p.Arguments, req.Arguments)
             ))
            return ResponseBaseModel.CreateInfo("Изменений нет");

        return ResponseBaseModel.CreateInfo("Команда успешно обновлена");
    }


    /// <inheritdoc/>
    public async Task<SyncDirectoryModelDB[]> GetSyncDirectoriesForConfig(int confId)
    {
        using ToolsAppContext context = await toolsDbFactory.CreateDbContextAsync();
        return await context.SyncDirectories.Where(x => x.ParentId == confId).ToArrayAsync();
    }

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> DeleteSyncDirectory(int syncDirectoryId)
    {
        using ToolsAppContext context = await toolsDbFactory.CreateDbContextAsync();

        if (1 > await context
           .SyncDirectories
           .Where(x => x.Id == syncDirectoryId)
           .ExecuteDeleteAsync())
            return ResponseBaseModel.CreateInfo("Синхронизация не существует");

        return ResponseBaseModel.CreateInfo("Синхронизация успешно удалена");
    }

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> UpdateOrCreateSyncDirectory(SyncDirectoryModelDB req)
    {
        using ToolsAppContext context = await toolsDbFactory.CreateDbContextAsync();

        if (req.Id < 1)
        {
            req.Parent = null;
            req.Id = 0;
            await context.AddAsync(req);
            await context.SaveChangesAsync();

            return ResponseBaseModel.CreateSuccess("Синхронизация успешно добавлена");
        }

        if (1 > await context
             .SyncDirectories
             .Where(x => x.Id == req.Id)
             .ExecuteUpdateAsync(set => set
             .SetProperty(p => p.Name, req.Name)
             .SetProperty(p => p.LocalDirectory, req.LocalDirectory)
             .SetProperty(p => p.RemoteDirectory, req.RemoteDirectory)
             ))
            return ResponseBaseModel.CreateInfo("Изменений нет");

        return ResponseBaseModel.CreateInfo("Синхронизация успешно обновлена");
    }
}