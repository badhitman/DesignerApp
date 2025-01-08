////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.commerce;

/// <summary>
/// NomenclaturesSelectReceive
/// </summary>
public class NomenclaturesSelectReceive(ICommerceService commerceRepo) : IResponseReceive<TPaginationRequestModel<NomenclaturesSelectRequestModel>?, TPaginationResponseModel<NomenclatureModelDB>?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.NomenclaturesSelectCommerceReceive;

    /// <inheritdoc/>
    public async Task<TPaginationResponseModel<NomenclatureModelDB>?> ResponseHandleAction(TPaginationRequestModel<NomenclaturesSelectRequestModel>? req)
    {
        ArgumentNullException.ThrowIfNull(req);
        return await commerceRepo.NomenclaturesSelect(req);
    }
}