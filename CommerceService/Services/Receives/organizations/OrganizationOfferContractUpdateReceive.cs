﻿////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Newtonsoft.Json;
using RemoteCallLib;
using SharedLib;

namespace Transmission.Receives.commerce;

/// <summary>
/// Organization offer-contract update (toggle)
/// </summary>
public class OrganizationOfferContractUpdateReceive(ICommerceService commerceRepo, ILogger<OrganizationOfferContractUpdateReceive> loggerRepo)
    : IResponseReceive<TAuthRequestModel<OrganizationOfferToggleModel>?, bool?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.OrganizationOfferContractUpdateOrCreateCommerceReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<bool?>> ResponseHandleAction(TAuthRequestModel<OrganizationOfferToggleModel>? req)
    {
        ArgumentNullException.ThrowIfNull(req?.Payload);
        loggerRepo.LogInformation($"call `{GetType().Name}`: {JsonConvert.SerializeObject(req)}");
        TResponseModel<bool> res = await commerceRepo.OrganizationOfferContractUpdate(req);
        return new()
        {
            Response = res.Response,
            Messages = res.Messages
        };
    }
}