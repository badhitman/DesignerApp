﻿////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.Extensions.Caching.Memory;
using Microsoft.EntityFrameworkCore;
using RemoteCallLib;
using IdentityLib;
using SharedLib;

namespace Transmission.Receives.web;

/// <summary>
/// Find user identity - receive
/// </summary>
public class GetUsersOfIdentityReceive(
    IDbContextFactory<IdentityAppDbContext> identityDbFactory,
    IMemoryCache cache)
    : IResponseReceive<string[]?, UserInfoModel[]?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.GetUsersOfIdentityReceive;

    static readonly TimeSpan _ts = TimeSpan.FromSeconds(5);

    /// <inheritdoc/>
    public async Task<TResponseModel<UserInfoModel[]?>> ResponseHandleAction(string[]? users_ids = null)
    {
        ArgumentNullException.ThrowIfNull(users_ids);
        users_ids = [.. users_ids.Where(x => !string.IsNullOrWhiteSpace(x))];
        TResponseModel<UserInfoModel[]?> res = new() { Response = [] };
        if (users_ids.Length == 0)
        {
            res.AddError("Пустой запрос");
            return res;
        }
        string[] find_users_ids = [.. users_ids.Where(x => x != GlobalStaticConstants.Roles.System)];
        if (find_users_ids.Length == 0)
            return res;

        string mem_token = $"{QueueName}-identity/{string.Join(",", find_users_ids)}";
        if (cache.TryGetValue(mem_token, out UserInfoModel[]? users_cache))
        {
            res.Response = users_cache;
            return res;
        }

        using IdentityAppDbContext identityContext = await identityDbFactory.CreateDbContextAsync();
        ApplicationUser[] users = await identityContext
            .Users
            .Where(x => find_users_ids.Contains(x.Id))
            .ToArrayAsync();

        if (users.Length == 0)
        {
            cache.Set(mem_token, Array.Empty<ApplicationUser>(), new MemoryCacheEntryOptions().SetAbsoluteExpiration(_ts));
            res.AddWarning("Не найдено ни одного пользователя");
            return res;
        }

        var users_roles = await identityContext
           .UserRoles
           .Where(x => find_users_ids.Contains(x.UserId))
           .Select(x => new { x.RoleId, x.UserId })
           .ToArrayAsync();

        EntryAltModel[] roles_names = users_roles.Length == 0
            ? []
            : await identityContext
                .Roles
                .Where(x => users_roles.Select(x => x.RoleId).Distinct().ToArray().Contains(x.Id))
                .Select(x => new EntryAltModel() { Id = x.Id, Name = x.Name })
                .ToArrayAsync();

        EntryAltTagModel[] claims = await identityContext
             .UserClaims
             .Where(x => find_users_ids.Contains(x.UserId) && x.ClaimType != null && x.ClaimType != "")
             .Select(x => new EntryAltTagModel() { Id = x.UserId, Name = x.ClaimType, Tag = x.ClaimValue })
             .ToArrayAsync();

        string[]? roles_for_user(string user_id)
        {
            return roles_names
                .Where(x => users_roles.Any(y => y.UserId == user_id && y.RoleId == x.Id))
                .Select(x => x.Name!)
                .Distinct()
                .ToArray();
        }

        UserInfoModel convert_user(ApplicationUser app_user)
        {
            return new()
            {
                GivenName = app_user.FirstName,
                Surname = app_user.LastName,
                UserId = app_user.Id,
                AccessFailedCount = app_user.AccessFailedCount,
                Email = app_user.Email,
                EmailConfirmed = app_user.EmailConfirmed,
                LockoutEnabled = app_user.LockoutEnabled,
                LockoutEnd = app_user.LockoutEnd,
                PhoneNumber = app_user.PhoneNumber,
                UserName = app_user.UserName,
                TelegramId = app_user.ChatTelegramId,
                Roles = [.. roles_for_user(app_user.Id)],
                Claims = [.. claims.Where(x => x.Id == app_user.Id).Select(x => new EntryAltModel() { Id = x.Id, Name = x.Name })]
            };
        }

        res.Response = users.Select(convert_user).ToArray();

        cache.Set(mem_token, res.Response, new MemoryCacheEntryOptions().SetAbsoluteExpiration(_ts));

        if (users_ids.Any(x => x == GlobalStaticConstants.Roles.System))
            res.Response = [.. res.Response.Union([UserInfoModel.BuildSystem()])];

        find_users_ids = [.. find_users_ids.Where(x => !res.Response.Any(y => y.UserId == x))];
        if (find_users_ids.Length != 0)
            res.AddWarning($"Некоторые пользователи не найдены: {string.Join(",", find_users_ids)}");

        return res;
    }
}