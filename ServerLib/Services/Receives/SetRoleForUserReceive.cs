////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RemoteCallLib;
using IdentityLib;
using SharedLib;

namespace Transmission.Receives.web;

/// <summary>
/// SetRoleForUserReceive
/// </summary>
public class SetRoleForUserReceive(IDbContextFactory<IdentityAppDbContext> identityDbFactory, ILogger<SetRoleForUserReceive> _logger)
    : IResponseReceive<SetRoleFoeUserRequestModel, TResponseModel<string[]>>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.SetRoleForUserOfIdentityReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<string[]>?> ResponseHandleAction(SetRoleFoeUserRequestModel? req)
    {
        ArgumentNullException.ThrowIfNull(req);
        _logger.LogInformation($"call `{GetType().Name}`: {JsonConvert.SerializeObject(req, GlobalStaticConstants.JsonSerializerSettings)}");
        TResponseModel<string[]> res = new();
        using IdentityAppDbContext identityContext = await identityDbFactory.CreateDbContextAsync();

        IQueryable<ApplicationRole> q = identityContext
            .UserRoles
            .Where(x => x.UserId == req.UserIdentityId)
            .Join(identityContext.Roles, jr => jr.RoleId, r => r.Id, (jr, r) => r)
            .AsQueryable();

        ApplicationRole[] roles = await q
            .ToArrayAsync();

        ApplicationRole? role_bd;
        if (req.Command && !roles.Any(x => x.Name?.Contains(req.RoleName, StringComparison.OrdinalIgnoreCase) == true))
        {
            role_bd = await identityContext
                .Roles
                .FirstOrDefaultAsync(x => x.NormalizedName == req.RoleName.ToUpper());

            if (role_bd is null)
            {
                role_bd = new ApplicationRole()
                {
                    NormalizedName = req.RoleName.ToUpper(),
                    Name = req.RoleName,
                };
                await identityContext.AddAsync(role_bd);
                await identityContext.SaveChangesAsync();
            }
            await identityContext.AddAsync(new IdentityUserRole<string>() { RoleId = role_bd.Id, UserId = req.UserIdentityId });
            await identityContext.SaveChangesAsync();
            res.Response = [.. roles.Select(x => x.Name).Union([req.RoleName])];
            res.AddSuccess($"Включён в роль: {role_bd.Name}");
        }
        else if (!req.Command && roles.Any(x => x.Name?.Contains(req.RoleName, StringComparison.OrdinalIgnoreCase) == true))
        {
            role_bd = roles.First(x => x.Name?.Contains(req.RoleName, StringComparison.OrdinalIgnoreCase) == true);
            identityContext.Remove(role_bd);
            await identityContext.SaveChangesAsync();
            res.Response = [.. roles.Select(x => x.Name).Where(x => x?.Equals(req.RoleName, StringComparison.OrdinalIgnoreCase) != true)];
            res.AddSuccess($"Исключён из роли: {req.RoleName}");
        }
        else
        {
            res.AddInfo("Изменения не требуются");
            res.Response = [.. roles.Select(x => x.Name)];
        }

        return res;
    }
}