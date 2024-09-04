////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;
using RemoteCallLib;
using SharedLib;
using DbcLib;

namespace Transmission.Receives.commerce;

/// <summary>
/// AddressOrganizationUpdateReceive
/// </summary>
public class AddressOrganizationUpdateReceive(IDbContextFactory<CommerceContext> commerceDbFactory)
    : IResponseReceive<AddressOrganizationBaseModel?, bool?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.AddressOrganizationUpdateCommerceReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<bool?>> ResponseHandleAction(AddressOrganizationBaseModel? req)
    {
        ArgumentNullException.ThrowIfNull(req);
        TResponseModel<bool?> res = new() { Response = false };
        using CommerceContext context = await commerceDbFactory.CreateDbContextAsync();

        if (req.Id < 1)
        {
            await context.AddAsync(new AddressOrganizationModelDB()
            {
                Address = req.Address,
                Name = req.Name,
                ParentId = req.ParentId,
                Contacts = req.Contacts,
                OrganizationId = req.OrganizationId,
            });
            await context.SaveChangesAsync();
            res.AddSuccess("Адрес добавлен");
            return res;
        }

        await context.AddressesOrganizations
            .Where(x => x.Id == req.Id)
            .ExecuteUpdateAsync(set => set
            .SetProperty(p => p.Address, req.Address)
            .SetProperty(p => p.Name, req.Name)
            .SetProperty(p => p.ParentId, req.ParentId)
            .SetProperty(p => p.Contacts, req.Contacts)
            .SetProperty(p => p.OrganizationId, req.OrganizationId));

        res.AddSuccess($"Обновление `{GetType().Name}` выполнено");
        return res;
    }
}