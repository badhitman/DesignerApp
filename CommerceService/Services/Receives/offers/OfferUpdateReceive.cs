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
/// OfferUpdateReceive
/// </summary>
public class OfferUpdateReceive(IDbContextFactory<CommerceContext> commerceDbFactory, ILogger<OfferUpdateReceive> loggerRepo)
    : IResponseReceive<TAuthRequestModel<OfferModelDB>?, TResponseModel<int>?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.OfferUpdateCommerceReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<int>?> ResponseHandleAction(TAuthRequestModel<OfferModelDB>? req)
    {
        ArgumentNullException.ThrowIfNull(req);
        loggerRepo.LogInformation($"call `{GetType().Name}`: {JsonConvert.SerializeObject(req, GlobalStaticConstants.JsonSerializerSettings)}");
        TResponseModel<int> res = new() { Response = 0 };

        if (!string.IsNullOrWhiteSpace(req.Payload.QuantitiesTemplate))
        {
            System.Collections.Immutable.ImmutableList<decimal> idss = req.Payload.QuantitiesTemplate.SplitToDecimalList();

            if (idss.Count == 0)
            {
                res.AddError("Формат доступных значений не корректный");
                return res;
            }
            req.Payload.QuantitiesTemplate = string.Join(" ", idss.Order());
        }

        using CommerceContext context = await commerceDbFactory.CreateDbContextAsync();
        DateTime dtu = DateTime.UtcNow;
        if (req.Payload.Id < 1)
        {
            req.Payload = new()
            {
                Name = req.Payload.Name,
                QuantitiesTemplate = req.Payload.QuantitiesTemplate,
                CreatedAtUTC = dtu,
                Description = req.Payload.Description,
                ShortName = req.Payload.ShortName,
                IsDisabled = req.Payload.IsDisabled,
                Multiplicity = req.Payload.Multiplicity,
                NomenclatureId = req.Payload.NomenclatureId,
                OfferUnit = req.Payload.OfferUnit,
                Price = req.Payload.Price,
                LastAtUpdatedUTC = dtu,
            };

            await context.AddAsync(req);
            await context.SaveChangesAsync();
            res.AddSuccess("Предложение добавлено");
            res.Response = req.Payload.Id;
            return res;
        }

        res.Response = await context.Offers
            .Where(x => x.Id == req.Payload.Id)
            .ExecuteUpdateAsync(set => set
            .SetProperty(p => p.Name, req.Payload.Name)
            .SetProperty(p => p.Description, req.Payload.Description)
            .SetProperty(p => p.QuantitiesTemplate, req.Payload.QuantitiesTemplate)
            .SetProperty(p => p.ShortName, req.Payload.ShortName)
            .SetProperty(p => p.IsDisabled, req.Payload.IsDisabled)
            .SetProperty(p => p.Multiplicity, req.Payload.Multiplicity)
            .SetProperty(p => p.NomenclatureId, req.Payload.NomenclatureId)
            .SetProperty(p => p.OfferUnit, req.Payload.OfferUnit)
            .SetProperty(p => p.Price, req.Payload.Price)
            .SetProperty(p => p.LastAtUpdatedUTC, dtu));

        res.AddSuccess($"Обновление `{GetType().Name}` выполнено");
        return res;
    }
}