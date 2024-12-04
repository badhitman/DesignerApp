////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RemoteCallLib;
using SharedLib;
using DbcLib;

namespace Transmission.Receives.commerce;

/// <summary>
/// Обновление номенклатуры
/// </summary>
public class NomenclatureUpdateReceive(ICommerceService commerceRepo, ILogger<NomenclatureUpdateReceive> loggerRepo)
    : IResponseReceive<NomenclatureModelDB?, int?>
{
    /// <summary>
    /// Обновление номенклатуры
    /// </summary>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.GoodsUpdateCommerceReceive;

    /// <summary>
    /// Обновление номенклатуры
    /// </summary>
    public async Task<TResponseModel<int?>> ResponseHandleAction(NomenclatureModelDB? req)
    {
        ArgumentNullException.ThrowIfNull(req);
        req.Name = req.Name.Trim();
        loggerRepo.LogInformation($"call `{GetType().Name}`: {JsonConvert.SerializeObject(req, GlobalStaticConstants.JsonSerializerSettings)}");
        TResponseModel<int> res = await commerceRepo.NomenclatureUpdate(req);
        return new()
        {
            Messages = res.Messages,
            Response = res.Response,
        };
    }
}