////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.Extensions.Caching.Memory;
using System.Net.Mail;
using RemoteCallLib;
using IdentityLib;
using SharedLib;

namespace Transmission.Receives.Identity;

/// <summary>
/// Получить пользователей из Identity по их Email
/// </summary>
public class GetUsersIdentityByEmailReceive(IIdentityTools IdentityRepo, IMemoryCache cache)
    : IResponseReceive<string[]?, TResponseModel<UserInfoModel[]>?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.GetUsersOfIdentityByEmailReceive;

    static readonly TimeSpan _ts = TimeSpan.FromSeconds(5);

    /// <inheritdoc/>
    public async Task<TResponseModel<UserInfoModel[]>?> ResponseHandleAction(string[]? users_emails = null)
    {
        ArgumentNullException.ThrowIfNull(users_emails);
        users_emails = [.. users_emails.Where(x => MailAddress.TryCreate(x, out _)).Select(x => x.ToUpper())];
        TResponseModel<UserInfoModel[]> res = new() { Response = [] };
        if (users_emails.Length == 0)
        {
            res.AddError("Пустой запрос");
            return res;
        }

        string mem_token = $"{QueueName}-identity-e/{string.Join(",", users_emails)}";
        if (cache.TryGetValue(mem_token, out UserInfoModel[]? users_cache))
        {
            res.Response = users_cache;
            return res;
        }
        res = await IdentityRepo.GetUsersIdentityByEmail(users_emails);

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