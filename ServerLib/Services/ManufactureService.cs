using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SharedLib;
using DbcLib;

namespace ServerLib;

/// <inheritdoc/>
public class ManufactureService(
    IDbContextFactory<MainDbAppContext> mainDbFactory,
    IUsersProfilesService usersProfilesRepo) : IManufactureService
{
    /// <inheritdoc/>
    public async Task<TResponseModel<ManageManufactureModelDB>> ReadManufactureConfig(int projectId, string? userId = null)
    {
        TResponseModel<ManageManufactureModelDB> res = new();
        TResponseModel<UserInfoModel?> user = await usersProfilesRepo.FindByIdAsync(userId);

        if (!user.Success() || user.Response is null)
        {
            res.AddRangeMessages(user.Messages);
            return res;
        }

        using MainDbAppContext context_forms = mainDbFactory.CreateDbContext();

        res.Response = await context_forms
            .Manufactures
            .FirstOrDefaultAsync(x => x.ProjectId == projectId && x.UserId == user.Response.UserId);

        if (res.Response is null)
        {
            string project_name = await context_forms
                        .Projects
                        .Where(x => x.Id == projectId)
                        .Select(x => x.Name)
                        .FirstAsync();

            res.Response = new ManageManufactureModelDB() { UserId = user.Response.UserId, Namespace = GlobalTools.TranslitToSystemName(project_name) };
            await context_forms.AddAsync(res.Response);
            await context_forms.SaveChangesAsync();
        }

        return res;
    }
}
