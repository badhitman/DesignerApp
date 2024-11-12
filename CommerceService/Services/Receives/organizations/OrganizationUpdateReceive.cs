////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;
using RemoteCallLib;
using SharedLib;
using DbcLib;
using Newtonsoft.Json;

namespace Transmission.Receives.commerce;

/// <summary>
/// Organization update or create
/// </summary>
public class OrganizationUpdateReceive(IDbContextFactory<CommerceContext> commerceDbFactory, ILogger<OrganizationUpdateReceive> loggerRepo)
    : IResponseReceive<TAuthRequestModel<OrganizationModelDB>?, int?>
{
    /// <inheritdoc/>
    public static string QueueName => GlobalStaticConstants.TransmissionQueues.OrganizationUpdateOrCreateCommerceReceive;

    /// <inheritdoc/>
    public async Task<TResponseModel<int?>> ResponseHandleAction(TAuthRequestModel<OrganizationModelDB>? req)
    {
        ArgumentNullException.ThrowIfNull(req?.Payload);
        loggerRepo.LogInformation($"call `{GetType().Name}`: {JsonConvert.SerializeObject(req)}");
        TResponseModel<int?> res = new() { Response = 0 };
        (bool IsValid, List<System.ComponentModel.DataAnnotations.ValidationResult> ValidationResults) = GlobalTools.ValidateObject(req.Payload);
        if (!IsValid)
        {
            res.Messages.InjectException(ValidationResults);
            return res;
        }
        using CommerceContext context = await commerceDbFactory.CreateDbContextAsync();
        IQueryable<OrganizationModelDB> _q = context
                .Organizations
                .Where(x => x.INN == req.Payload.INN || x.OGRN == req.Payload.OGRN || (x.BankBIC == req.Payload.BankBIC && x.CurrentAccount == req.Payload.CurrentAccount && x.CorrespondentAccount == req.Payload.CorrespondentAccount))
                .AsQueryable();

        if (req.Payload.Id < 1)
        {
            OrganizationModelDB? duple = await _q
                .FirstOrDefaultAsync();

            if (duple is not null)
            {
                IQueryable<UserOrganizationModelDB> sq = context
                    .OrganizationsUsers
                    .Where(x => x.UserPersonIdentityId == req.SenderActionUserId && x.OrganizationId == duple.Id);

                if (!string.IsNullOrWhiteSpace(req.SenderActionUserId) && req.SenderActionUserId != GlobalStaticConstants.Roles.System && sq.Any())
                {
                    await context.AddAsync(new UserOrganizationModelDB()
                    {
                        LastAtUpdatedUTC = DateTime.UtcNow,
                        UserPersonIdentityId = req.SenderActionUserId,
                        OrganizationId = duple.Id,
                    });
                    await context.SaveChangesAsync();
                    res.AddSuccess($"Вы добавлены к управлению компанией");
                }

                res.AddWarning($"Компания уже существует");
                return res;
            }

            req.Payload.NewINN = req.Payload.INN;
            req.Payload.NewOGRN = req.Payload.OGRN;
            req.Payload.NewBankBIC = req.Payload.BankBIC;
            req.Payload.NewCorrespondentAccount = req.Payload.CorrespondentAccount;
            req.Payload.NewCurrentAccount = req.Payload.CurrentAccount;
            req.Payload.NewBankName = req.Payload.BankName;
            req.Payload.NewName = req.Payload.Name;
            req.Payload.NewLegalAddress = req.Payload.LegalAddress;
            req.Payload.NewKPP = req.Payload.KPP;
            req.Payload.LastAtUpdatedUTC = DateTime.UtcNow;

            await context.AddAsync(req.Payload);
            await context.SaveChangesAsync();
            await context.AddAsync(new UserOrganizationModelDB()
            {
                LastAtUpdatedUTC = DateTime.UtcNow,
                UserPersonIdentityId = req.SenderActionUserId,
                OrganizationId = req.Payload.Id,
            });
            await context.SaveChangesAsync();
            res.AddSuccess($"Компания создана");

            res.Response = req.Payload.Id;
        }
        else
        {
            DateTime lud = DateTime.UtcNow;
            OrganizationModelDB org_db = await context.Organizations.FirstAsync(x => x.Id == req.Payload.Id);

            IQueryable<OrganizationModelDB> q = context
                .Organizations
                .Where(x => x.Id == org_db.Id);

            if (org_db.Name != req.Payload.Name)
                await q.ExecuteUpdateAsync(set => set.SetProperty(p => p.NewName, req.Payload.Name));
            else if (org_db.Name != org_db.NewName)
                await q.ExecuteUpdateAsync(set => set.SetProperty(p => p.NewName, ""));

            if (org_db.LegalAddress != req.Payload.LegalAddress)
                await q.ExecuteUpdateAsync(set => set.SetProperty(p => p.NewLegalAddress, req.Payload.LegalAddress));
            else if (org_db.LegalAddress != org_db.NewLegalAddress)
                await q.ExecuteUpdateAsync(set => set.SetProperty(p => p.NewLegalAddress, ""));

            if (org_db.CurrentAccount != req.Payload.CurrentAccount)
                await q.ExecuteUpdateAsync(set => set.SetProperty(p => p.NewCurrentAccount, req.Payload.CurrentAccount));
            else if (org_db.CurrentAccount != org_db.NewCurrentAccount)
                await q.ExecuteUpdateAsync(set => set.SetProperty(p => p.NewCurrentAccount, ""));

            if (org_db.BankBIC != req.Payload.BankBIC)
                await q.ExecuteUpdateAsync(set => set.SetProperty(p => p.NewBankBIC, req.Payload.BankBIC));
            else if (org_db.BankBIC != org_db.NewBankBIC)
                await q.ExecuteUpdateAsync(set => set.SetProperty(p => p.NewBankBIC, ""));

            if (org_db.BankName != req.Payload.BankName)
                await q.ExecuteUpdateAsync(set => set.SetProperty(p => p.NewBankName, req.Payload.BankName));
            else if (org_db.BankName != org_db.BankName)
                await q.ExecuteUpdateAsync(set => set.SetProperty(p => p.NewBankName, ""));

            if (org_db.INN != req.Payload.INN)
                await q.ExecuteUpdateAsync(set => set.SetProperty(p => p.NewINN, req.Payload.INN));
            else if (org_db.INN != org_db.INN)
                await q.ExecuteUpdateAsync(set => set.SetProperty(p => p.NewINN, ""));

            if (org_db.OGRN != req.Payload.OGRN)
                await q.ExecuteUpdateAsync(set => set.SetProperty(p => p.NewOGRN, req.Payload.OGRN));
            else if (org_db.OGRN != org_db.OGRN)
                await q.ExecuteUpdateAsync(set => set.SetProperty(p => p.NewOGRN, ""));

            if (org_db.CorrespondentAccount != req.Payload.CorrespondentAccount)
                await q.ExecuteUpdateAsync(set => set.SetProperty(p => p.NewCorrespondentAccount, req.Payload.CorrespondentAccount));
            else if (org_db.CorrespondentAccount != org_db.CorrespondentAccount)
                await q.ExecuteUpdateAsync(set => set.SetProperty(p => p.NewCorrespondentAccount, ""));

            if (org_db.KPP != req.Payload.KPP)
                await q.ExecuteUpdateAsync(set => set.SetProperty(p => p.NewKPP, req.Payload.KPP));
            else if (org_db.KPP != org_db.KPP)
                await q.ExecuteUpdateAsync(set => set.SetProperty(p => p.NewKPP, ""));

            await q.ExecuteUpdateAsync(set => set.SetProperty(p => p.LastAtUpdatedUTC, lud));

            if (org_db.Email != req.Payload.Email)
            {
                await context
                    .Organizations
                    .Where(x => x.Id == org_db.Id)
                    .ExecuteUpdateAsync(set => set
                    .SetProperty(p => p.LastAtUpdatedUTC, lud)
                    .SetProperty(p => p.Email, req.Payload.Email));
                res.AddSuccess("Email изменён");
            }
            if (org_db.Phone != req.Payload.Phone)
            {
                await context
                    .Organizations
                    .Where(x => x.Id == org_db.Id)
                    .ExecuteUpdateAsync(set => set
                    .SetProperty(p => p.LastAtUpdatedUTC, lud)
                    .SetProperty(p => p.Phone, req.Payload.Phone));
                res.AddSuccess("Phone изменён");
            }
            if (org_db.IsDisabled != req.Payload.IsDisabled)
            {
                await context
                    .Organizations
                    .Where(x => x.Id == org_db.Id)
                    .ExecuteUpdateAsync(set => set
                    .SetProperty(p => p.IsDisabled, req.Payload.IsDisabled));

                res.AddSuccess(req.Payload.IsDisabled ? "Организация успешно отключена" : "Организация успешно включена");
            }
        }

        return res;
    }
}