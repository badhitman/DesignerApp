////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.Identity;

/// <summary>
/// Возвращает флаг, указывающий, действителен ли данный password для указанного userId
/// </summary>
/// <returns>
/// true, если указанный password соответствует для userId, в противном случае значение false.
/// </returns>
public class CheckUserPasswordReceive(IIdentityTools idRepo)
    : IResponseReceive<IdentityPasswordModel?, ResponseBaseModel?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.CheckUserPasswordReceive;

    /// <summary>
    /// Возвращает флаг, указывающий, действителен ли данный password для указанного userId
    /// </summary>
    /// <returns>
    /// true, если указанный password соответствует для userId, в противном случае значение false.
    /// </returns>
    public async Task<ResponseBaseModel?> ResponseHandleAction(IdentityPasswordModel? req)
    {
        ArgumentNullException.ThrowIfNull(req);
        return await idRepo.CheckUserPassword(req);
    }
}