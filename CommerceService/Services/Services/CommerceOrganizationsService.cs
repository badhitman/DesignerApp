////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SharedLib;
using DbcLib;

namespace CommerceService;

/// <summary>
/// Organizations
/// </summary>
public partial class CommerceImplementService : ICommerceService
{
    /// <inheritdoc/>
    public async Task<OrganizationContractorModel[]> ContractorsOrganizationsFind(ContractorsOrganizationsRequestModel req)
    {
        using CommerceContext context = await commerceDbFactory.CreateDbContextAsync();
        IQueryable<OrganizationContractorModel> q = req.OfferFilter is null
            ? context.ContractorsOrganizations.Where(x => x.OfferId == null)
            : context.ContractorsOrganizations.Where(x => x.OfferId == null || x.OfferId == req.OfferFilter);

        if (req.OrganizationsFilter is not null && req.OrganizationsFilter.Length != 0)
            q = q.Where(x => req.OrganizationsFilter.Contains(x.OrganizationId));

        return await q.ToArrayAsync();
    }

    /// <inheritdoc/>
    public async Task<TResponseModel<bool>> OrganizationOfferContractUpdate(TAuthRequestModel<OrganizationOfferToggleModel> req)
    {
        using CommerceContext context = await commerceDbFactory.CreateDbContextAsync();

        if (req.Payload.OfferId < 1)
            req.Payload.OfferId = null;

        if (req.Payload.SetValue)
        {
            if (await context.ContractorsOrganizations.AnyAsync(x => x.OrganizationId == req.Payload.OrganizationId && x.OfferId == req.Payload.OfferId))
            {
                return new()
                {
                    Messages = [new() { Text = "Контракт уже установлен", TypeMessage = ResultTypesEnum.Info }],
                    Response = false,
                };
            }
            await context.ContractorsOrganizations.AddAsync(new() { OfferId = req.Payload.OfferId, OrganizationId = req.Payload.OrganizationId });

            try
            {
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                string msg = $"Ошибка создания контракта [{JsonConvert.SerializeObject(req)}]";
                loggerRepo.LogError(ex, msg);
                return new()
                {
                    Messages = [new() { Text = $"{msg}: {ex.Message}", TypeMessage = ResultTypesEnum.Error }],
                    Response = false,
                };
            }

            return new()
            {
                Messages = [new() { Text = "Контракт создан", TypeMessage = ResultTypesEnum.Success }],
                Response = true,
            };
        }
        else
        {
            if (!await context.ContractorsOrganizations.AnyAsync(x => x.OrganizationId == req.Payload.OrganizationId && x.OfferId == req.Payload.OfferId))
            {
                return new()
                {
                    Messages = [new() { Text = "Контракта нет", TypeMessage = ResultTypesEnum.Info }],
                    Response = false,
                };
            }
            return new()
            {
                Messages = [new() { Text = "Контракт удалён", TypeMessage = ResultTypesEnum.Success }],
                Response = await context.ContractorsOrganizations.Where(x => x.OrganizationId == req.Payload.OrganizationId && x.OfferId == req.Payload.OfferId).ExecuteDeleteAsync() > 0,
            };
        }
    }

    /// <inheritdoc/>
    public async Task<TResponseModel<AddressOrganizationModelDB[]>> AddressesOrganizationsRead(int[] organizationsIds)
    {
        TResponseModel<AddressOrganizationModelDB[]> res = new();
        using CommerceContext context = await commerceDbFactory.CreateDbContextAsync();

        res.Response = await context
            .AddressesOrganizations
            .Where(x => organizationsIds.Any(y => y == x.Id))
            .ToArrayAsync();

        return res;
    }

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> AddressOrganizationDelete(int address_id)
    {
        ResponseBaseModel res = new();

        using CommerceContext context = await commerceDbFactory.CreateDbContextAsync();

        int count = await context
            .OrdersDocuments
            .CountAsync(x => context.TabsAddressesForOrders.Any(y => y.OrderDocumentId == x.Id && y.AddressOrganizationId == address_id));

        if (count != 0)
            res.AddError($"Адрес используется в заказах: {count} шт.");

        //count = await context
        //    .OrdersDocuments
        //    .CountAsync(x => context.TabsAddressesForOrders.Any(y => y.OrderDocumentId == x.Id && y.DeliveryAddressId == req));

        //if (count != 0)
        //    res.AddError($"Адрес указан в доставке: {count} шт.");

        if (!res.Success())
            return res;

        await context.AddressesOrganizations.Where(x => x.Id == address_id).ExecuteDeleteAsync();
        res.AddSuccess("Команда успешно выполнена");

        return res;
    }

    /// <inheritdoc/>
    public async Task<TResponseModel<int>> AddressOrganizationUpdate(AddressOrganizationBaseModel req)
    {
        TResponseModel<int> res = new();
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

        res.Response = await context.AddressesOrganizations
                        .Where(x => x.Id == req.Id)
                        .ExecuteUpdateAsync(set => set
                        //.SetProperty(p => p.OrganizationId, req.OrganizationId)
                        .SetProperty(p => p.Address, req.Address)
                        .SetProperty(p => p.Name, req.Name)
                        .SetProperty(p => p.ParentId, req.ParentId)
                        .SetProperty(p => p.Contacts, req.Contacts));

        res.AddSuccess($"Обновление `{GetType().Name}` выполнено");
        return res;
    }

    /// <inheritdoc/>
    public async Task<TResponseModel<bool>> OrganizationSetLegal(OrganizationLegalModel org)
    {
        TResponseModel<bool> res = new() { Response = false };
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

        org_db.LegalAddress = org.LegalAddress;
        org_db.NewLegalAddress = null;

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

    /// <inheritdoc/>
    public async Task<TResponseModel<OrganizationModelDB[]>> OrganizationsRead(int[] req)
    {
        TResponseModel<OrganizationModelDB[]> res = new();
        using CommerceContext context = await commerceDbFactory.CreateDbContextAsync();

        res.Response = await context
            .Organizations
            .Where(x => req.Any(y => y == x.Id))
            .Include(x => x.Addresses)
            .Include(x => x.Users)
            .ToArrayAsync();

        return res;
    }

    /// <inheritdoc/>
    public async Task<TResponseModel<TPaginationResponseModel<OrganizationModelDB>>> OrganizationsSelect(TPaginationRequestAuthModel<OrganizationsSelectRequestModel> req)
    {
        TResponseModel<UserInfoModel[]?> res = await webTransmissionRepo.GetUsersIdentity([req.SenderActionUserId]);
        if (!res.Success() || res.Response?.Length != 1)
            return new()
            {
                Messages = res.Messages,
            };
        UserInfoModel actor = res.Response[0];
        if (req.PageSize < 10)
            req.PageSize = 10;

        using CommerceContext context = await commerceDbFactory.CreateDbContextAsync();
        IQueryable<OrganizationModelDB> q = context.Organizations.AsQueryable();
        if (!actor.IsAdmin)
            q = q.Where(x => context.OrganizationsUsers.Any(y => y.OrganizationId == x.Id && y.UserStatus > UsersOrganizationsStatusesEnum.None && y.UserPersonIdentityId == req.SenderActionUserId));

        if (req.Payload.AfterDateUpdate is not null)
            q = q.Where(x => x.LastAtUpdatedUTC >= req.Payload.AfterDateUpdate);

        if (!string.IsNullOrWhiteSpace(req.Payload.ForUserIdentityId))
            q = q.Where(x => context.OrganizationsUsers.Any(y => y.OrganizationId == x.Id && y.UserPersonIdentityId == req.Payload.ForUserIdentityId));

        q = req.SortingDirection == VerticalDirectionsEnum.Up
            ? q.OrderBy(x => x.Name)
            : q.OrderByDescending(x => x.Name);

        IQueryable<OrganizationModelDB> pq = q
            .Skip(req.PageNum * req.PageSize)
            .Take(req.PageSize);

        var extQ = pq
            .Include(x => x.Addresses)
            .Include(x => x.Users)
            .Include(x => x.Contractors!)
            .ThenInclude(x => x.Offer);

        return new()
        {
            Response = new()
            {
                PageNum = req.PageNum,
                PageSize = req.PageSize,
                SortingDirection = req.SortingDirection,
                SortBy = req.SortBy,
                TotalRowsCount = await q.CountAsync(),
                Response = req.Payload.IncludeExternalData ? [.. await extQ.ToArrayAsync()] : [.. await pq.ToArrayAsync()]
            }
        };
    }

    /// <inheritdoc/>
    public async Task<TResponseModel<int>> OrganizationUpdate(TAuthRequestModel<OrganizationModelDB> req)
    {
        TResponseModel<int> res = new() { Response = 0 };
        (bool IsValid, List<System.ComponentModel.DataAnnotations.ValidationResult> ValidationResults) = GlobalTools.ValidateObject(req.Payload);
        if (!IsValid)
        {
            res.Messages.InjectException(ValidationResults);
            return res;
        }
        using CommerceContext context = await commerceDbFactory.CreateDbContextAsync();
        OrganizationModelDB? duple;
        UserInfoModel actor = default!;
        await Task.WhenAll([
            Task.Run(async () =>
            {
                TResponseModel<UserInfoModel[]?> userFind = await webTransmissionRepo.GetUsersIdentity([req.SenderActionUserId]);
                actor = userFind.Response!.First();
                }),
            Task.Run(async () => { duple = await context.Organizations.FirstOrDefaultAsync(x => x.INN == req.Payload.INN || x.OGRN == req.Payload.OGRN || (x.BankBIC == req.Payload.BankBIC && x.CurrentAccount == req.Payload.CurrentAccount && x.CorrespondentAccount == req.Payload.CorrespondentAccount)); })
            ]);

        duple = await context
                .Organizations
                .FirstOrDefaultAsync(x => x.INN == req.Payload.INN || x.OGRN == req.Payload.OGRN || (x.BankBIC == req.Payload.BankBIC && x.CurrentAccount == req.Payload.CurrentAccount && x.CorrespondentAccount == req.Payload.CorrespondentAccount));

        if (req.Payload.Id < 1)
        {
            if (duple is not null)
            {
                UserOrganizationModelDB? sq = await context
                    .OrganizationsUsers
                    .FirstOrDefaultAsync(x => x.UserPersonIdentityId == req.SenderActionUserId && x.OrganizationId == duple.Id);

                if (!string.IsNullOrWhiteSpace(req.SenderActionUserId) && req.SenderActionUserId != GlobalStaticConstants.Roles.System && sq is not null)
                {
                    await context.AddAsync(new UserOrganizationModelDB()
                    {
                        LastAtUpdatedUTC = DateTime.UtcNow,
                        UserPersonIdentityId = req.SenderActionUserId,
                        OrganizationId = duple.Id,
                        UserStatus = UsersOrganizationsStatusesEnum.None,
                    });

                    await Task.WhenAll([
                        context.SaveChangesAsync(),
                        Task.Run(async () =>
                        {
                            await webTransmissionRepo.SendEmail(new SendEmailRequestModel()
                            {
                                TextMessage = $"В компанию `{sq}` добавлен сотрудник: {actor}",
                                Email = "*",
                                Subject = "Новый сотрудник" }, false); })
                        ]);

                    res.AddSuccess($"Вы добавлены к управлению компанией, но требуется подтверждение администратором");
                }
                else
                    res.AddWarning($"Компания уже существует. Требуется подтверждение администратором");
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


    /// <inheritdoc/>
    public async Task<TResponseModel<int>> UserOrganizationUpdate(TAuthRequestModel<UserOrganizationModelDB> req)
    {
        TResponseModel<int> res = new() { Response = 0 };
        using CommerceContext context = await commerceDbFactory.CreateDbContextAsync();

        if (req.Payload.Id < 1)
        {
            UserOrganizationModelDB? dl = await context.OrganizationsUsers
                .FirstOrDefaultAsync(x => x.OrganizationId == req.Payload.OrganizationId && x.UserPersonIdentityId == req.Payload.UserPersonIdentityId);

            if (dl is not null)
            {
                res.AddInfo($"Пользователь уже существует: {dl.UserStatus.DescriptionInfo()}");
                return res;
            }

            await context.AddAsync(req.Payload);
            await context.SaveChangesAsync();
            res.AddSuccess("Пользователь добавлен");
            res.Response = req.Payload.Id;
            return res;
        }

        res.Response = await context.OrganizationsUsers
                        .Where(x => x.Id == req.Payload.Id)
                        .ExecuteUpdateAsync(set => set
                        .SetProperty(p => p.UserStatus, req.Payload.UserStatus)
                        .SetProperty(p => p.LastAtUpdatedUTC, DateTime.UtcNow));

        res.AddSuccess($"Обновление `{nameof(UserOrganizationUpdate)}` выполнено");
        return res;
    }

    /// <inheritdoc/>
    public async Task<TResponseModel<UserOrganizationModelDB[]>> UsersOrganizationsRead(int[] req)
    {
        TResponseModel<UserOrganizationModelDB[]> res = new();
        using CommerceContext context = await commerceDbFactory.CreateDbContextAsync();
        res.Response = await context
            .OrganizationsUsers
            .Where(x => req.Any(y => y == x.Id))
            .Include(x => x.Organization!)
            .ThenInclude(x => x.Users)
            .ToArrayAsync();

        return res;
    }

    /// <inheritdoc/>
    public async Task<TResponseModel<TPaginationResponseModel<UserOrganizationModelDB>>> UsersOrganizationsSelect(TPaginationRequestAuthModel<UsersOrganizationsStatusesRequest> req)
    {
        if (req.PageSize < 10)
            req.PageSize = 10;

        using CommerceContext context = await commerceDbFactory.CreateDbContextAsync();

        IQueryable<UserOrganizationModelDB> q = req.Payload.UsersOrganizationsStatusesFilter is not null && req.Payload.UsersOrganizationsStatusesFilter.Length > 0
            ? context.OrganizationsUsers.Where(x => req.Payload.UsersOrganizationsStatusesFilter.Any(y => y == x.UserStatus))
            : context.OrganizationsUsers.AsQueryable();

        if (req.Payload.AfterDateUpdate is not null)
            q = q.Where(x => x.LastAtUpdatedUTC >= req.Payload.AfterDateUpdate);

        if (req.Payload.OrganizationsFilter is not null && req.Payload.OrganizationsFilter.Length != 0)
        {
            q = q.Where(x => req.Payload.OrganizationsFilter.Any(y => y == x.OrganizationId));
            q = req.SortingDirection == VerticalDirectionsEnum.Up
            ? q.OrderBy(x => x.UserStatus)
            : q.OrderByDescending(x => x.UserStatus);
        }
        else
        {
            q = req.SortingDirection == VerticalDirectionsEnum.Up
            ? q.OrderBy(x => x.Organization!.Name)
            : q.OrderByDescending(x => x.Organization!.Name);
        }



        IQueryable<UserOrganizationModelDB> pq = q
            .Skip(req.PageNum * req.PageSize)
            .Take(req.PageSize);

        var extQ = pq
            .Include(x => x.Organization!)
            .ThenInclude(x => x.Users);

        return new()
        {
            Response = new()
            {
                PageNum = req.PageNum,
                PageSize = req.PageSize,
                SortingDirection = req.SortingDirection,
                SortBy = req.SortBy,
                TotalRowsCount = await q.CountAsync(),
                Response = req.Payload.IncludeExternalData ? await extQ.ToListAsync() : await pq.ToListAsync()
            }
        };
    }
}