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
    public async Task<TResponseModel<CodeGeneratorConfigModel>> ReadManufactureConfig(int projectId, string? userId = null)
    {
        TResponseModel<CodeGeneratorConfigModel> res = new();
        TResponseModel<UserInfoModel?> call_user = await usersProfilesRepo.FindByIdAsync(userId);

        if (!call_user.Success() || call_user.Response is null)
        {
            res.AddRangeMessages(call_user.Messages);
            return res;
        }

        using MainDbAppContext context_forms = mainDbFactory.CreateDbContext();



        ManageManufactureModelDB? manufacture = await context_forms.Manufactures.FirstOrDefaultAsync(x => x.ProjectId == projectId && x.UserId == call_user.Response.UserId);

        if(manufacture is null)
        {

        }

        throw new NotImplementedException();
    }
}
