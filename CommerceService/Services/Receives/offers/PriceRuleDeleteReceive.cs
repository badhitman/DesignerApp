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
/// PriceRuleDeleteReceive
/// </summary>
public class PriceRuleDeleteReceive(IDbContextFactory<CommerceContext> commerceDbFactory, ILogger<OfferUpdateReceive> loggerRepo)
    : IResponseReceive<int?, bool?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.PriceRuleDeleteCommerceReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<bool?>> ResponseHandleAction(int? id)
    {
        ArgumentNullException.ThrowIfNull(id);
        loggerRepo.LogInformation($"call `{GetType().Name}`: {JsonConvert.SerializeObject(id, GlobalStaticConstants.JsonSerializerSettings)}");
        using CommerceContext context = await commerceDbFactory.CreateDbContextAsync();
        bool exe_delete = await context.PricesRules.Where(x => x.Id == id).ExecuteDeleteAsync() > 0;
        TResponseModel<bool?> res = new() { Response = exe_delete };

        if (exe_delete)
            res.AddSuccess("Правило ценообразования успешно удалено");
        else
            res.AddInfo("Правило отсутствует");

        return res;
    }
}