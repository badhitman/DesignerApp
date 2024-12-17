////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.commerce;

/// <summary>
/// OrganizationsSelectReceive
/// </summary>
public class OrganizationsSelectReceive(ICommerceService commerceRepo)
    : IResponseReceive<TPaginationRequestAuthModel<OrganizationsSelectRequestModel>?, TPaginationResponseModel<OrganizationModelDB>?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.OrganizationsSelectCommerceReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<TPaginationResponseModel<OrganizationModelDB>?>> ResponseHandleAction(TPaginationRequestAuthModel<OrganizationsSelectRequestModel>? req)
    {
        ArgumentNullException.ThrowIfNull(req);
        TResponseModel<TPaginationResponseModel<OrganizationModelDB>> res = await commerceRepo.OrganizationsSelect(req);
        return new()
        {
            Response = res.Response,
            Messages = res.Messages,
        };
    }
}