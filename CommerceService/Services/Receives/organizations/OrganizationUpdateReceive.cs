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
    : IResponseReceive<TAuthRequestModel<OrganizationModelDB>?, int?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.OrganizationUpdateOrCreateCommerceReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<int?>> ResponseHandleAction(TAuthRequestModel<OrganizationModelDB>? org)
    {
        ArgumentNullException.ThrowIfNull(org?.Payload);
        TResponseModel<int?> res = new() { Response = 0 };
        using CommerceContext context = await commerceDbFactory.CreateDbContextAsync();

        if (org.Payload.Id < 1)
        {
            OrganizationModelDB? duble = await context
                .Organizations
                .FirstOrDefaultAsync(x => (x.INN == org.Payload.INN || x.OGRN == org.Payload.OGRN) || (x.BankBIC == org.Payload.BankBIC && x.CurrentAccount == org.Payload.CurrentAccount && x.CorrespondentAccount == org.Payload.CorrespondentAccount));

            if (duble is not null)
            {
                //await context.AddAsync(new UserOrganizationModelDB()
                //{
                //    LastAtUpdatedUTC = DateTime.UtcNow
                //});
                res.AddError($"Для получения доступа к компании обратитесь к администратору");
                return res;
            }

            org.Payload.NewINN = org.Payload.INN;
            org.Payload.NewOGRN = org.Payload.OGRN;
            org.Payload.NewBankBIC = org.Payload.BankBIC;
            org.Payload.NewCorrespondentAccount = org.Payload.CorrespondentAccount;
            org.Payload.NewCurrentAccount = org.Payload.CurrentAccount;
            org.Payload.NewBankName = org.Payload.BankName;
            org.Payload.NewName = org.Payload.Name;
            org.Payload.NewLegalAddress = org.Payload.LegalAddress;
            org.Payload.NewKPP = org.Payload.KPP;
            org.Payload.LastAtUpdatedUTC = DateTime.UtcNow;

            await context.AddAsync(org);
            await context.SaveChangesAsync();
            res.Response = org.Payload.Id;
        }
        else
        {
            DateTime lud = DateTime.UtcNow;
            OrganizationModelDB org_db = await context.Organizations.FirstAsync(x => x.Id == org.Payload.Id);

            IQueryable<OrganizationModelDB> q = context
                .Organizations
                .Where(x => x.Id == org_db.Id);

            if (org_db.Name != org.Payload.Name)
                await q.ExecuteUpdateAsync(set => set.SetProperty(p => p.NewName, org.Payload.Name));
            else if (org_db.Name != org_db.NewName)
                await q.ExecuteUpdateAsync(set => set.SetProperty(p => p.NewName, ""));

            if (org_db.LegalAddress != org.Payload.LegalAddress)
                await q.ExecuteUpdateAsync(set => set.SetProperty(p => p.NewLegalAddress, org.Payload.LegalAddress));
            else if (org_db.LegalAddress != org_db.NewLegalAddress)
                await q.ExecuteUpdateAsync(set => set.SetProperty(p => p.NewLegalAddress, ""));

            if (org_db.CurrentAccount != org.Payload.CurrentAccount)
                await q.ExecuteUpdateAsync(set => set.SetProperty(p => p.NewCurrentAccount, org.Payload.CurrentAccount));
            else if (org_db.CurrentAccount != org_db.NewCurrentAccount)
                await q.ExecuteUpdateAsync(set => set.SetProperty(p => p.NewCurrentAccount, ""));

            if (org_db.BankBIC != org.Payload.BankBIC)
                await q.ExecuteUpdateAsync(set => set.SetProperty(p => p.NewBankBIC, org.Payload.BankBIC));
            else if (org_db.BankBIC != org_db.NewBankBIC)
                await q.ExecuteUpdateAsync(set => set.SetProperty(p => p.NewBankBIC, ""));

            if (org_db.BankName != org.Payload.BankName)
                await q.ExecuteUpdateAsync(set => set.SetProperty(p => p.NewBankName, org.Payload.BankName));
            else if (org_db.BankName != org_db.BankName)
                await q.ExecuteUpdateAsync(set => set.SetProperty(p => p.NewBankName, ""));

            if (org_db.INN != org.Payload.INN)
                await q.ExecuteUpdateAsync(set => set.SetProperty(p => p.NewINN, org.Payload.INN));
            else if (org_db.INN != org_db.INN)
                await q.ExecuteUpdateAsync(set => set.SetProperty(p => p.NewINN, ""));

            if (org_db.OGRN != org.Payload.OGRN)
                await q.ExecuteUpdateAsync(set => set.SetProperty(p => p.NewOGRN, org.Payload.OGRN));
            else if (org_db.OGRN != org_db.OGRN)
                await q.ExecuteUpdateAsync(set => set.SetProperty(p => p.NewOGRN, ""));

            if (org_db.CorrespondentAccount != org.Payload.CorrespondentAccount)
                await q.ExecuteUpdateAsync(set => set.SetProperty(p => p.NewCorrespondentAccount, org.Payload.CorrespondentAccount));
            else if (org_db.CorrespondentAccount != org_db.CorrespondentAccount)
                await q.ExecuteUpdateAsync(set => set.SetProperty(p => p.NewCorrespondentAccount, ""));

            if (org_db.KPP != org.Payload.KPP)
                await q.ExecuteUpdateAsync(set => set.SetProperty(p => p.NewKPP, org.Payload.KPP));
            else if (org_db.KPP != org_db.KPP)
                await q.ExecuteUpdateAsync(set => set.SetProperty(p => p.NewKPP, ""));

            await q.ExecuteUpdateAsync(set => set.SetProperty(p => p.LastAtUpdatedUTC, lud));

            if (org_db.Email != org.Payload.Email)
            {
                await context
                    .Organizations
                    .Where(x => x.Id == org_db.Id)
                    .ExecuteUpdateAsync(set => set
                    .SetProperty(p => p.LastAtUpdatedUTC, lud)
                    .SetProperty(p => p.Email, org.Payload.Email));
                res.AddSuccess("Email изменён");
            }
            if (org_db.Phone != org.Payload.Phone)
            {
                await context
                    .Organizations
                    .Where(x => x.Id == org_db.Id)
                    .ExecuteUpdateAsync(set => set
                    .SetProperty(p => p.LastAtUpdatedUTC, lud)
                    .SetProperty(p => p.Phone, org.Payload.Phone));
                res.AddSuccess("Phone изменён");
            }
            if (org_db.IsDisabled != org.Payload.IsDisabled)
            {
                await context
                    .Organizations
                    .Where(x => x.Id == org_db.Id)
                    .ExecuteUpdateAsync(set => set
                    .SetProperty(p => p.IsDisabled, org.Payload.IsDisabled));

                res.AddSuccess(org.Payload.IsDisabled ? "Организация успешно отключена" : "Организация успешно включена");
            }
        }

        return res;
    }
}