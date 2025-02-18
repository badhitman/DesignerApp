////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;
using SharedLib;
using DbcLib;

namespace ToolsMauiApp;

/// <summary>
/// ToolsAppManager
/// </summary>
public class ToolsAppManager(IDbContextFactory<ToolsAppContext> toolsDbFactory) : IToolsAppManager
{
    public async Task<ApiRestConfigModelDB[]> GetAllConfigurations()
    {
        using ToolsAppContext context = await toolsDbFactory.CreateDbContextAsync();
        return await context.Configurations.ToArrayAsync();
    }
}
