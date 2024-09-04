////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;
using RemoteCallLib;
using SharedLib;
using DbcLib;

namespace Transmission.Receives.commerce;

/// <summary>
/// Organization update or create
/// </summary>
public class OrganizationUpdateReceive(IDbContextFactory<CommerceContext> commerceDbFactory)
    : IResponseReceive<OrganizationModelDB?, int?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.OrganizationUpdateOrCreateCommerceReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<int?>> ResponseHandleAction(OrganizationModelDB? org)
    {
        ArgumentNullException.ThrowIfNull(org);
        TResponseModel<int?> res = new() { Response = 0 };
        using CommerceContext context = await commerceDbFactory.CreateDbContextAsync();

        if (org.Id < 1)
        {
            org.NewBankBIC = org.BankBIC;
            org.NewBankName = org.BankName;
            org.NewINN = org.INN;
            org.NewOGRN = org.OGRN;
            org.NewName = org.Name;
            org.NewLegalAddress = org.LegalAddress;
            org.NewCorrespondentAccount = org.CorrespondentAccount;
            org.NewKPP = org.KPP;
            org.LastAtUpdatedUTC = DateTime.UtcNow;

            await context.AddAsync(org);
            await context.SaveChangesAsync();
            res.Response = org.Id;
        }
        else
        {
            DateTime lud = DateTime.UtcNow;
            OrganizationModelDB org_db = await context.Organizations.FirstAsync(x => x.Id == org.Id);

            IQueryable<OrganizationModelDB> q = context
                .Organizations
                .Where(x => x.Id == org_db.Id);

            if (org_db.Name != org.Name)
                await q.ExecuteUpdateAsync(set => set.SetProperty(p => p.NewName, org.Name));
            else if (org_db.Name != org_db.NewName)
                await q.ExecuteUpdateAsync(set => set.SetProperty(p => p.NewName, ""));

            if (org_db.LegalAddress != org.LegalAddress)
                await q.ExecuteUpdateAsync(set => set.SetProperty(p => p.NewLegalAddress, org.LegalAddress));
            else if (org_db.LegalAddress != org_db.NewLegalAddress)
                await q.ExecuteUpdateAsync(set => set.SetProperty(p => p.NewLegalAddress, ""));

            if (org_db.CurrentAccount != org.CurrentAccount)
                await q.ExecuteUpdateAsync(set => set.SetProperty(p => p.NewCurrentAccount, org.CurrentAccount));
            else if (org_db.CurrentAccount != org_db.NewCurrentAccount)
                await q.ExecuteUpdateAsync(set => set.SetProperty(p => p.NewCurrentAccount, ""));

            if (org_db.BankBIC != org.BankBIC)
                await q.ExecuteUpdateAsync(set => set.SetProperty(p => p.NewBankBIC, org.BankBIC));
            else if (org_db.BankBIC != org_db.NewBankBIC)
                await q.ExecuteUpdateAsync(set => set.SetProperty(p => p.NewBankBIC, ""));

            if (org_db.BankName != org.BankName)
                await q.ExecuteUpdateAsync(set => set.SetProperty(p => p.NewBankName, org.BankName));
            else if (org_db.BankName != org_db.BankName)
                await q.ExecuteUpdateAsync(set => set.SetProperty(p => p.NewBankName, ""));

            if (org_db.INN != org.INN)
                await q.ExecuteUpdateAsync(set => set.SetProperty(p => p.NewINN, org.INN));
            else if (org_db.INN != org_db.INN)
                await q.ExecuteUpdateAsync(set => set.SetProperty(p => p.NewINN, ""));

            if (org_db.OGRN != org.OGRN)
                await q.ExecuteUpdateAsync(set => set.SetProperty(p => p.NewOGRN, org.OGRN));
            else if (org_db.OGRN != org_db.OGRN)
                await q.ExecuteUpdateAsync(set => set.SetProperty(p => p.NewOGRN, ""));

            if (org_db.CorrespondentAccount != org.CorrespondentAccount)
                await q.ExecuteUpdateAsync(set => set.SetProperty(p => p.NewCorrespondentAccount, org.CorrespondentAccount));
            else if (org_db.CorrespondentAccount != org_db.CorrespondentAccount)
                await q.ExecuteUpdateAsync(set => set.SetProperty(p => p.NewCorrespondentAccount, ""));

            if (org_db.KPP != org.KPP)
                await q.ExecuteUpdateAsync(set => set.SetProperty(p => p.NewKPP, org.KPP));
            else if (org_db.KPP != org_db.KPP)
                await q.ExecuteUpdateAsync(set => set.SetProperty(p => p.NewKPP, ""));

            await q.ExecuteUpdateAsync(set => set.SetProperty(p => p.LastAtUpdatedUTC, lud));

            if (org_db.Email != org.Email)
            {
                await context
                    .Organizations
                    .Where(x => x.Id == org_db.Id)
                    .ExecuteUpdateAsync(set => set
                    .SetProperty(p => p.LastAtUpdatedUTC, lud)
                    .SetProperty(p => p.Email, org.Email));
                res.AddSuccess("Email изменён");
            }
            if (org_db.Phone != org.Phone)
            {
                await context
                    .Organizations
                    .Where(x => x.Id == org_db.Id)
                    .ExecuteUpdateAsync(set => set
                    .SetProperty(p => p.LastAtUpdatedUTC, lud)
                    .SetProperty(p => p.Phone, org.Phone));
                res.AddSuccess("Phone изменён");
            }
            if (org_db.IsDisabled != org.IsDisabled)
            {
                await context
                    .Organizations
                    .Where(x => x.Id == org_db.Id)
                    .ExecuteUpdateAsync(set => set
                    .SetProperty(p => p.IsDisabled, org.IsDisabled));

                res.AddSuccess(org.IsDisabled ? "Адрес отключён" : "Адрес включён");
            }
        }

        return res;
    }
}