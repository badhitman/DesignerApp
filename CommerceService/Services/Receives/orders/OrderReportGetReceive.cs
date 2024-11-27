////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.commerce;

/// <summary>
/// OrderReportGetReceive
/// </summary>
public class OrderReportGetReceive(ICommerceService commRepo)
: IResponseReceive<TAuthRequestModel<int>?, FileAttachModel?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.OrderReportGetCommerceReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<FileAttachModel?>> ResponseHandleAction(TAuthRequestModel<int>? req)
    {
        ArgumentNullException.ThrowIfNull(req);

        TResponseModel<FileAttachModel> res = await commRepo.GetOrderReportFile(req);
        return new()
        {
            Messages = res.Messages,
            Response = res.Response,
        };
    }
}
