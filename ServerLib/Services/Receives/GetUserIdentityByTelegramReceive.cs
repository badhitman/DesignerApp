////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.Extensions.Caching.Memory;
using Microsoft.EntityFrameworkCore;
using RemoteCallLib;
using IdentityLib;
using SharedLib;

namespace Transmission.Receives.web;

/// <summary>
/// Find user identity by telegram - receive
/// </summary>
public class GetUserIdentityByTelegramReceive(
    IDbContextFactory<IdentityAppDbContext> identityDbFactory,
    IWebRemoteTransmissionService webRepo,
    IMemoryCache cache)
    : IResponseReceive<long[], TResponseModel<UserInfoModel[]>>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.GetUsersOfIdentityByTelegramIdsReceive;

    static readonly TimeSpan _ts = TimeSpan.FromSeconds(5);

    /// <inheritdoc/>
    public async Task<TResponseModel<UserInfoModel[]>?> ResponseHandleAction(long[]? tg_ids = null)
    {
        ArgumentNullException.ThrowIfNull(tg_ids);
        tg_ids = [.. tg_ids.Where(x => x != 0)];
        TResponseModel<UserInfoModel[]> response = new() { Response = [] };
        if (tg_ids.Length == 0)
        {
            response.AddError("Пустой запрос");
            return response;
        }

        string mem_token = $"{QueueName}-tg/{string.Join(",", tg_ids)}";
        if (cache.TryGetValue(mem_token, out UserInfoModel[]? users_cache))
        {
            response.Response = users_cache;
            return response;
        }

        using IdentityAppDbContext identityContext = await identityDbFactory.CreateDbContextAsync();
        ApplicationUser[] users = await identityContext
            .Users
            .Where(x => tg_ids.Any(y => y == x.ChatTelegramId))
            .ToArrayAsync();

        if (users.Length == 0)
        {
            cache.Set(mem_token, Array.Empty<ApplicationUser>(), new MemoryCacheEntryOptions().SetAbsoluteExpiration(_ts));
            response.AddWarning("Не найдено ни одного пользователя");
            return response;
        }

        string[] users_ids = [.. users.Select(x => x.Id)];

        TResponseModel<UserInfoModel[]?> res_find_users_identity = await webRepo.GetUsersIdentity(users_ids);
        if (!res_find_users_identity.Success())
        {
            response.AddRangeMessages(res_find_users_identity.Messages);
            return response;
        }

        if (res_find_users_identity.Response is null || res_find_users_identity.Response.Length == 0)
        {
            response.AddError("Не найдены пользователи");
            return response;
        }
        response.Response = res_find_users_identity.Response;
        cache.Set(mem_token, response.Response, new MemoryCacheEntryOptions().SetAbsoluteExpiration(_ts));

        tg_ids = [.. tg_ids.Where(x => !response.Response.Any(y => y.TelegramId == x))];
        if (tg_ids.Length != 0)
            response.AddInfo($"Некоторые пользователи (Telegram) не найдены: {string.Join(",", tg_ids)}");

        return response;
    }
}