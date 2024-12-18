﻿////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SharedLib;
using System.Security.Claims;

namespace IdentityLib;

/// <summary>
/// IdentityTools
/// </summary>
public class IdentityTools(IDbContextFactory<IdentityAppDbContext> identityDbFactory)
{
    /// <summary>
    /// Установить пользователю Claim[TelegramId, FirstName, LastName, PhoneNum]
    /// </summary>
    public async Task<bool> ClaimsUpdateForUser(ApplicationUser app_user)
    {
        app_user.FirstName ??= "";
        app_user.NormalizedFirstNameUpper = app_user.FirstName.ToUpper();
        app_user.LastName ??= "";
        app_user.NormalizedLastNameUpper = app_user.LastName.ToUpper();

        bool res = false;
        using IdentityAppDbContext identityContext = await identityDbFactory.CreateDbContextAsync();
        string chat_tg_id = app_user.ChatTelegramId?.ToString() ?? "0";
        IdentityUserClaim<string>[] claims_bd;
        int[] claims_ids;
        if (chat_tg_id != "0")
        {
            claims_bd = await identityContext.UserClaims.Where(x => x.ClaimType == GlobalStaticConstants.TelegramIdClaimName && x.ClaimValue == chat_tg_id).ToArrayAsync();
            claims_ids = [.. claims_bd.Where(x => x.UserId != app_user.Id).Select(x => x.Id)];
            if (claims_ids.Length != 0)
            {
                res = 0 != await identityContext.UserClaims.Where(x => claims_ids.Contains(x.Id)).ExecuteDeleteAsync() || res;
            }

            if (!claims_bd.Any(x => x.UserId == app_user.Id))
            {
                await identityContext.AddAsync(new IdentityUserClaim<string>()
                {
                    ClaimType = GlobalStaticConstants.TelegramIdClaimName,
                    ClaimValue = app_user.ChatTelegramId.ToString(),
                    UserId = app_user.Id,
                });
                res = 0 != await identityContext.SaveChangesAsync() || res;
            }
        }
        else
        {
            res = 0 != await identityContext
                .UserClaims
                .Where(x => x.ClaimType == GlobalStaticConstants.TelegramIdClaimName && x.UserId == app_user.Id)
                .ExecuteDeleteAsync() || res;
        }

        claims_bd = await identityContext.UserClaims.Where(x => x.ClaimType == ClaimTypes.GivenName && x.UserId == app_user.Id).ToArrayAsync();
        if (string.IsNullOrWhiteSpace(app_user.FirstName))
        {
            if (claims_bd.Length != 0)
            {
                claims_ids = claims_bd.Select(x => x.Id).ToArray();
                res = 0 != await identityContext.UserClaims.Where(x => claims_ids.Contains(x.Id)).ExecuteDeleteAsync() || res;
            }
        }
        else
        {
            IdentityUserClaim<string> fe;
            IOrderedEnumerable<IdentityUserClaim<string>> qo = claims_bd.OrderBy(x => x.Id);

            if (claims_bd.Length == 0)
            {
                await identityContext.AddAsync(new IdentityUserClaim<string>()
                {
                    ClaimType = ClaimTypes.GivenName,
                    ClaimValue = app_user.FirstName ?? "",
                    UserId = app_user.Id
                });
                res = 0 != await identityContext.SaveChangesAsync() || res;
            }
            else if (claims_bd.Length > 1)
            {
                fe = qo.First();
                claims_ids = [.. qo.Skip(1).Select(x => x.Id)];
                res = 0 != await identityContext.UserClaims.Where(x => claims_ids.Contains(x.Id)).ExecuteDeleteAsync() || res;
            }
            else
            {
                fe = qo.First();
                if (fe.ClaimValue != app_user.FirstName)
                {
                    res = 0 != await identityContext
                                        .UserClaims
                                        .Where(x => x.Id == fe.Id)
                                        .ExecuteUpdateAsync(set => set.SetProperty(p => p.ClaimValue, app_user.FirstName)) || res;
                }
            }
        }

        claims_bd = await identityContext.UserClaims.Where(x => x.ClaimType == ClaimTypes.Surname && x.UserId == app_user.Id).ToArrayAsync();
        IOrderedEnumerable<IdentityUserClaim<string>> oq = claims_bd.OrderBy(x => x.Id);
        if (string.IsNullOrWhiteSpace(app_user.FirstName))
        {
            if (claims_bd.Length != 0)
            {
                claims_ids = claims_bd.Select(x => x.Id).ToArray();
                res = 0 != await identityContext.UserClaims.Where(x => claims_ids.Contains(x.Id)).ExecuteDeleteAsync() || res;
            }
        }
        else
        {
            if (claims_bd.Length == 0)
            {
                await identityContext.AddAsync(new IdentityUserClaim<string>()
                {
                    ClaimType = ClaimTypes.Surname,
                    ClaimValue = app_user.LastName ?? "",
                    UserId = app_user.Id
                });
                res = 0 != await identityContext.SaveChangesAsync() || res;
            }
            else if (claims_bd.Length > 1)
            {
                claims_ids = [.. oq.Skip(1).Select(x => x.Id)];
                res = 0 != await identityContext.UserClaims.Where(x => claims_ids.Contains(x.Id)).ExecuteDeleteAsync() || res;
            }
            else if (claims_bd[0].ClaimValue != app_user.FirstName)
            {
                res = 0 != await identityContext
                    .UserClaims
                    .Where(x => x.Id == oq.First().Id)
                    .ExecuteUpdateAsync(set => set.SetProperty(p => p.ClaimValue, app_user.LastName)) || res;
            }
        }

        claims_bd = await identityContext.UserClaims.Where(x => x.ClaimType == ClaimTypes.MobilePhone && x.UserId == app_user.Id).ToArrayAsync();
        IOrderedEnumerable<IdentityUserClaim<string>> ot = claims_bd.OrderBy(x => x.Id);
        if (string.IsNullOrWhiteSpace(app_user.PhoneNumber))
        {
            if (claims_bd.Length != 0)
            {
                claims_ids = claims_bd.Select(x => x.Id).ToArray();
                res = 0 != await identityContext.UserClaims.Where(x => claims_ids.Contains(x.Id)).ExecuteDeleteAsync() || res;
            }
        }
        else
        {
            if (claims_bd.Length == 0)
            {
                await identityContext.AddAsync(new IdentityUserClaim<string>()
                {
                    ClaimType = ClaimTypes.MobilePhone,
                    ClaimValue = app_user.PhoneNumber ?? "",
                    UserId = app_user.Id
                });
                res = 0 != await identityContext.SaveChangesAsync() || res;
            }
            else if (claims_bd.Length > 1)
            {
                claims_ids = [.. ot.Skip(1).Select(x => x.Id)];
                res = 0 != await identityContext.UserClaims.Where(x => claims_ids.Contains(x.Id)).ExecuteDeleteAsync() || res;
            }
            else if (claims_bd[0].ClaimValue != app_user.PhoneNumber)
            {
                res = 0 != await identityContext
                    .UserClaims
                    .Where(x => x.Id == ot.First().Id)
                    .ExecuteUpdateAsync(set => set.SetProperty(p => p.ClaimValue, app_user.PhoneNumber)) || res;
            }
        }

        return res;
    }
}