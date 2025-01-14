////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.Extensions.Caching.Memory;
using RemoteCallLib;
using IdentityLib;
using SharedLib;

namespace IdentityService.Services.Receives.users;

/// <summary>
/// Получить пользователей из Identity по их идентификаторам
/// </summary>
public class GetUsersOfIdentityReceive(IIdentityTools identityRepo, IMemoryCache cache)
    : IResponseReceive<string[]?, TResponseModel<UserInfoModel[]>?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.GetUsersOfIdentityReceive;

    static readonly TimeSpan _ts = TimeSpan.FromSeconds(5);

    /// <inheritdoc/>
    public async Task<TResponseModel<UserInfoModel[]>?> ResponseHandleAction(string[]? users_ids = null)
    {
        ArgumentNullException.ThrowIfNull(users_ids);
        users_ids = [.. users_ids.Where(x => !string.IsNullOrWhiteSpace(x)).Distinct()];
        TResponseModel<UserInfoModel[]> res = new();
        if (users_ids.Length == 0)
        {
            res.AddError("Пустой запрос");
            return res;
        }
        string[] find_users_ids = [.. users_ids.Where(x => x != GlobalStaticConstants.Roles.System).Order()];

        string mem_token = $"{QueueName}-identity/{string.Join(",", find_users_ids)}";
        if (cache.TryGetValue(mem_token, out UserInfoModel[]? users_cache))
        {
            res.Response = users_cache;
            return res;
        }
        res = await identityRepo.GetUsersOfIdentity(users_ids);

        if (res.Response is null || res.Response.Length == 0)
        {
            cache.Set(mem_token, Array.Empty<ApplicationUser>(), new MemoryCacheEntryOptions().SetAbsoluteExpiration(_ts));
            res.AddWarning("Не найдено ни одного пользователя");
            return res;
        }

        cache.Set(mem_token, res.Response, new MemoryCacheEntryOptions().SetAbsoluteExpiration(_ts));

        return res;
    }
}