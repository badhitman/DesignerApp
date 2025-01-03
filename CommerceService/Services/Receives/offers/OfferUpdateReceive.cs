﻿////////////////////////////////////////////////
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
    : IResponseReceive<OfferModelDB, TResponseModel<int>>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.OfferUpdateCommerceReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<int>?> ResponseHandleAction(OfferModelDB? req)
    {
        ArgumentNullException.ThrowIfNull(req);
        loggerRepo.LogInformation($"call `{GetType().Name}`: {JsonConvert.SerializeObject(req, GlobalStaticConstants.JsonSerializerSettings)}");
        TResponseModel<int> res = new() { Response = 0 };

        if (!string.IsNullOrWhiteSpace(req.QuantitiesTemplate))
        {
            var idss = req.QuantitiesTemplate.SplitToDecimalList();

            if (idss.Count == 0)
            {
                res.AddError("Формат доступных значений не корректный");
                return res;
            }
            req.QuantitiesTemplate = string.Join(" ", idss.Order());
        }

        using CommerceContext context = await commerceDbFactory.CreateDbContextAsync();
        DateTime dtu = DateTime.UtcNow;
        if (req.Id < 1)
        {
            req = new()
            {
                Name = req.Name,
                QuantitiesTemplate = req.QuantitiesTemplate,
                CreatedAtUTC = dtu,
                Description = req.Description,
                ShortName = req.ShortName,
                IsDisabled = req.IsDisabled,
                Multiplicity = req.Multiplicity,
                NomenclatureId = req.NomenclatureId,
                OfferUnit = req.OfferUnit,
                Price = req.Price,
                LastAtUpdatedUTC = dtu,
            };

            await context.AddAsync(req);
            await context.SaveChangesAsync();
            res.AddSuccess("Предложение добавлено");
            res.Response = req.Id;
            return res;
        }

        res.Response = await context.Offers
            .Where(x => x.Id == req.Id)
            .ExecuteUpdateAsync(set => set
            .SetProperty(p => p.Name, req.Name)
            .SetProperty(p => p.Description, req.Description)
            .SetProperty(p => p.QuantitiesTemplate, req.QuantitiesTemplate)
            .SetProperty(p => p.ShortName, req.ShortName)
            .SetProperty(p => p.IsDisabled, req.IsDisabled)
            .SetProperty(p => p.Multiplicity, req.Multiplicity)
            .SetProperty(p => p.NomenclatureId, req.NomenclatureId)
            .SetProperty(p => p.OfferUnit, req.OfferUnit)
            .SetProperty(p => p.Price, req.Price)
            .SetProperty(p => p.LastAtUpdatedUTC, dtu));

        res.AddSuccess($"Обновление `{GetType().Name}` выполнено");
        return res;
    }
}