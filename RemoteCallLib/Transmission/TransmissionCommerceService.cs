////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using SharedLib;

namespace RemoteCallLib;

/// <summary>
/// TransmissionCommerceService
/// </summary>
public class TransmissionCommerceService(IRabbitClient rabbitClient) : ICommerceRemoteTransmissionService
{
    /// <inheritdoc/>
    public async Task<TResponseModel<OrganizationModelDB[]?>> OrganizationsRead(int[] req)
        => await rabbitClient.MqRemoteCall<OrganizationModelDB[]?>(GlobalStaticConstants.TransmissionQueues.OrganizationsReadCommerceReceive, req);

    /// <inheritdoc/>
    public async Task<TResponseModel<TPaginationResponseModel<OrganizationModelDB>?>> OrganizationsSelect(TPaginationRequestModel<OrganizationsSelectRequestModel> req)
        => await rabbitClient.MqRemoteCall<TPaginationResponseModel<OrganizationModelDB>?>(GlobalStaticConstants.TransmissionQueues.OrganizationsSelectCommerceReceive, req);

    /// <inheritdoc/>
    public async Task<TResponseModel<int?>> OrganizationUpdate(OrganizationModelDB org)
        => await rabbitClient.MqRemoteCall<int?>(GlobalStaticConstants.TransmissionQueues.OrganizationUpdateOrCreateCommerceReceive, org);
}