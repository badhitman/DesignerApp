////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;
using RemoteCallLib;
using SharedLib;
using DbcLib;

namespace Transmission.Receives.commerce;

/// <summary>
/// Organization set legal
/// </summary>
public class OrganizationSetLegalReceive(IDbContextFactory<CommerceContext> commerceDbFactory)
    : IResponseReceive<OrganizationLegalModel?, bool?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.OrganizationSetLegalCommerceReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<bool?>> ResponseHandleAction(OrganizationLegalModel? org)
    {
        ArgumentNullException.ThrowIfNull(org);
        TResponseModel<bool?> res = new() { Response = false };
        using CommerceContext context = await commerceDbFactory.CreateDbContextAsync();
        OrganizationModelDB? org_db = await context.Organizations.FirstOrDefaultAsync(x => x.Id == org.Id);
        if (org_db is null)
        {
            res.AddError("Не найдена организация");
            return res;
        }

        org_db.BankBIC = org.BankBIC;
        org_db.NewBankBIC = null;

        org_db.BankName = org.BankName;
        org_db.NewBankName = null;

        org_db.INN = org.INN;
        org_db.NewINN = null;

        org_db.OGRN = org.OGRN;
        org_db.NewOGRN = null;

        org_db.CorrespondentAccount = org.CorrespondentAccount;
        org_db.NewCorrespondentAccount = null;

        org_db.CurrentAccount = org.CurrentAccount;
        org_db.NewCurrentAccount = null;

        org_db.KPP = org.KPP;
        org_db.NewKPP = null;

        org_db.Name = org.Name;
        org_db.NewName = null;

        org_db.Email = org.Email;
        org_db.Phone = org.Phone;
        org_db.IsDisabled = org.IsDisabled;
        org_db.LastAtUpdatedUTC = DateTime.UtcNow;

        context.Update(org_db);
        await context.SaveChangesAsync();
        res.Response = true;
        res.AddSuccess("Данные успешно сохранены");

        return res;
    }
}
