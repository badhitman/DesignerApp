////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using System.Net.Mail;
using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.Identity;

/// <summary>
/// Поиск пользователя по Email
/// </summary>
public class FindUserByEmailReceive(IIdentityTools idRepo)
    : IResponseReceive<string?, TResponseModel<UserInfoModel>?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.FindUserByEmailReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<UserInfoModel>?> ResponseHandleAction(string? req)
    {
        if (string.IsNullOrWhiteSpace(req) || !MailAddress.TryCreate(req, out _))
            throw new ArgumentNullException(nameof(req));

        return await idRepo.FindByEmailAsync(req);
    }
}