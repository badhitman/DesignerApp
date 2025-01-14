////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.Extensions.Caching.Memory;
using RemoteCallLib;
using IdentityLib;
using SharedLib;

namespace IdentityService.Services.Receives.users;

/// <summary>
/// Find user identity by telegram - receive
/// </summary>
public class GetUserIdentityByTelegramReceive(IIdentityTransmission IdentityRepo, IMemoryCache cache)
    : IResponseReceive<long[]?, TResponseModel<UserInfoModel[]>?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.GetUsersOfIdentityByTelegramIdsReceive;

    static readonly TimeSpan _ts = TimeSpan.FromSeconds(5);

    /// <inheritdoc/>
    public async Task<TResponseModel<UserInfoModel[]>?> ResponseHandleAction(long[]? tg_ids = null)
    {
        ArgumentNullException.ThrowIfNull(tg_ids);
        tg_ids = [.. tg_ids.Where(x => x != 0)];
        TResponseModel<UserInfoModel[]> response = new();
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

        response = await IdentityRepo.GetUserIdentityByTelegram(tg_ids);

        if (response.Response is null || response.Response.Length == 0)
        {
            cache.Set(mem_token, Array.Empty<ApplicationUser>(), new MemoryCacheEntryOptions().SetAbsoluteExpiration(_ts));
            response.AddWarning("Не найдено ни одного пользователя");
            return response;
        }

        cache.Set(mem_token, response.Response, new MemoryCacheEntryOptions().SetAbsoluteExpiration(_ts));

        return response;
    }
}