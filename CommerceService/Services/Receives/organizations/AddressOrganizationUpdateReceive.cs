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
/// AddressOrganizationUpdateReceive
/// </summary>
public class AddressOrganizationUpdateReceive(IDbContextFactory<CommerceContext> commerceDbFactory, ILogger<AddressOrganizationUpdateReceive> loggerRepo)
    : IResponseReceive<AddressOrganizationBaseModel?, int?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.AddressOrganizationUpdateCommerceReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<int?>> ResponseHandleAction(AddressOrganizationBaseModel? req)
    {
        ArgumentNullException.ThrowIfNull(req);
        loggerRepo.LogInformation($"call `{GetType().Name}`: {JsonConvert.SerializeObject(req, GlobalStaticConstants.JsonSerializerSettings)}");
        TResponseModel<int?> res = new();
        using CommerceContext context = await commerceDbFactory.CreateDbContextAsync();

        if (req.Id < 1)
        {
            AddressOrganizationModelDB add = new()
            {
                Address = req.Address,
                Name = req.Name,
                ParentId = req.ParentId,
                Contacts = req.Contacts,
                OrganizationId = req.OrganizationId,
            };
            await context.AddAsync(add);
            await context.SaveChangesAsync();
            res.AddSuccess("Адрес добавлен");
            res.Response = add.Id;
            return res;
        }

        try
        {
            res.Response = await context.AddressesOrganizations
                        .Where(x => x.Id == req.Id)
                        .ExecuteUpdateAsync(set => set
                        //.SetProperty(p => p.OrganizationId, req.OrganizationId)
                        .SetProperty(p => p.Address, req.Address)
                        .SetProperty(p => p.Name, req.Name)
                        .SetProperty(p => p.ParentId, req.ParentId)
                        .SetProperty(p => p.Contacts, req.Contacts));
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }



        res.AddSuccess($"Обновление `{GetType().Name}` выполнено");
        return res;
    }
}