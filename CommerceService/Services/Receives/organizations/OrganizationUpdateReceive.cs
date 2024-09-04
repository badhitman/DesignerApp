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
            if (org_db.BankBIC == org.BankBIC &&
                org_db.BankName == org.BankName &&
                org_db.INN == org.INN &&
                org_db.OGRN == org.OGRN &&
                org_db.CorrespondentAccount == org.CorrespondentAccount &&
                org_db.KPP == org.KPP)
            {
                res.AddInfo("Изменений в юридических данных нет");
            }
            else
            {
                await context
                    .Organizations
                    .Where(x => x.Id == org_db.Id)
                    .ExecuteUpdateAsync(set => set
                    .SetProperty(p => p.NewBankBIC, org.BankBIC)
                    .SetProperty(p => p.NewBankName, org.BankName)
                    .SetProperty(p => p.NewINN, org.INN)
                    .SetProperty(p => p.NewOGRN, org.OGRN)
                    .SetProperty(p => p.NewCorrespondentAccount, org.CorrespondentAccount)
                    .SetProperty(p => p.NewKPP, org.KPP)
                    .SetProperty(p => p.LastAtUpdatedUTC, lud));

                res.AddWarning("Запрос на изменение сформирован");
            }

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
                    .SetProperty(p => p.LastAtUpdatedUTC, lud)
                    .SetProperty(p => p.IsDisabled, org.IsDisabled));

                res.AddSuccess(org.IsDisabled ? "Адрес отключён" : "Адрес включён");
            }
        }

        return res;
    }
}