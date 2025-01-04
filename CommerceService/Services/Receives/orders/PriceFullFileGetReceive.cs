////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.commerce;

/// <summary>
/// PriceFullFileGetReceive
/// </summary>
public class PriceFullFileGetReceive(ICommerceService commRepo)
    : IResponseReceive<object?, FileAttachModel?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.PriceFullFileGetCommerceReceive;

    /// <inheritdoc/>
    public async Task<FileAttachModel?> ResponseHandleAction(object? req)
    {
        return await commRepo.GetFullPriceFile();
    }
}
