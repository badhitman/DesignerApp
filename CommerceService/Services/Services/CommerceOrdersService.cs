﻿////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore.Storage;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Packaging;
using Microsoft.EntityFrameworkCore;
using HtmlGenerator.html5.textual;
using HtmlGenerator.html5.tables;
using HtmlGenerator.html5.areas;
using DocumentFormat.OpenXml;
using System.Globalization;
using Newtonsoft.Json;
using System.Text;
using System.Data;
using SharedLib;
using DbcLib;

namespace CommerceService;

/// <summary>
/// Commerce
/// </summary>
public partial class CommerceImplementService(
    IDbContextFactory<CommerceContext> commerceDbFactory,
    IWebRemoteTransmissionService webTransmissionRepo,
    IHelpdeskRemoteTransmissionService hdRepo,
    ITelegramRemoteTransmissionService tgRepo,
    ILogger<CommerceImplementService> loggerRepo,
    WebConfigModel _webConf,
    ISerializeStorageRemoteTransmissionService StorageTransmissionRepo) : ICommerceService
{
    private static readonly CultureInfo cultureInfo = new("ru-RU");

    /// <inheritdoc/>
    public async Task<TResponseModel<OrderDocumentModelDB[]>> OrdersByIssuesGet(OrdersByIssuesSelectRequestModel req)
    {
        using CommerceContext context = await commerceDbFactory.CreateDbContextAsync();

        IQueryable<OrderDocumentModelDB> q = context
            .OrdersDocuments
            .AsQueryable();

        if (req.IssueIds.Length != 0)
            q = q.Where(x => req.IssueIds.Any(y => y == x.HelpdeskId));
        else
            return new()
            {
                Response = [],
                Messages = [new() { TypeMessage = ResultTypesEnum.Error, Text = "Запрос не может быть пустым" }]
            };

        Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<OrderDocumentModelDB, NomenclatureModelDB?> inc_query = q
            .Include(x => x.Organization)
            .Include(x => x.AddressesTabs!)
            .ThenInclude(x => x.AddressOrganization)
            .Include(x => x.AddressesTabs!)
            .ThenInclude(x => x.Rows!)
            .ThenInclude(x => x.Offer!)
            .ThenInclude(x => x.Nomenclature);

        return new()
        {
            Response = req.IncludeExternalData ? [.. await inc_query.ToArrayAsync()] : [.. await q.ToArrayAsync()],
        };
    }

    /// <inheritdoc/>
    public async Task<TResponseModel<OrderDocumentModelDB[]>> OrdersRead(int[] req)
    {
        TResponseModel<OrderDocumentModelDB[]> res = new();

        using CommerceContext context = await commerceDbFactory.CreateDbContextAsync();

        IQueryable<OrderDocumentModelDB> q = context
            .OrdersDocuments
            .Where(x => req.Any(y => x.Id == y));

        res.Response = await q
            .Include(x => x.AddressesTabs!)
            .ThenInclude(x => x.AddressOrganization)
            .Include(x => x.AddressesTabs!)
            .ThenInclude(x => x.Rows!)
            .ThenInclude(x => x.Offer!)
            .ThenInclude(x => x.Nomenclature)
            .ToArrayAsync();

        return res;
    }

    /// <inheritdoc/>
    public async Task<TResponseModel<TPaginationResponseModel<OrderDocumentModelDB>>> OrdersSelect(TPaginationRequestModel<TAuthRequestModel<OrdersSelectRequestModel>> req)
    {
        if (req.PageSize < 10)
            req.PageSize = 10;

        using CommerceContext context = await commerceDbFactory.CreateDbContextAsync();

        IQueryable<OrderDocumentModelDB> q = context
            .OrdersDocuments
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(req.Payload.SenderActionUserId) && !req.Payload.SenderActionUserId.Equals(GlobalStaticConstants.Roles.System))
            q = q.Where(x => x.AuthorIdentityUserId == req.Payload.SenderActionUserId);

        if (req.Payload.Payload.OrganizationFilter.HasValue && req.Payload.Payload.OrganizationFilter.Value != 0)
            q = q.Where(x => x.OrganizationId == req.Payload.Payload.OrganizationFilter);

        if (req.Payload.Payload.AddressForOrganizationFilter.HasValue && req.Payload.Payload.AddressForOrganizationFilter.Value != 0)
            q = q.Where(x => context.TabsAddressesForOrders.Any(y => y.OrderDocumentId == x.Id && y.AddressOrganizationId == req.Payload.Payload.AddressForOrganizationFilter));

        if (req.Payload.Payload.OfferFilter.HasValue && req.Payload.Payload.OfferFilter.Value != 0)
            q = q.Where(x => context.RowsOfOrdersDocuments.Any(y => y.OrderDocumentId == x.Id && y.OfferId == req.Payload.Payload.OfferFilter));

        if (req.Payload.Payload.NomenclatureFilter.HasValue && req.Payload.Payload.NomenclatureFilter.Value != 0)
            q = q.Where(x => context.RowsOfOrdersDocuments.Any(y => y.OrderDocumentId == x.Id && y.NomenclatureId == req.Payload.Payload.NomenclatureFilter));

        if (req.Payload.Payload.AfterDateUpdate is not null)
            q = q.Where(x => x.LastAtUpdatedUTC >= req.Payload.Payload.AfterDateUpdate || (x.LastAtUpdatedUTC == DateTime.MinValue && x.CreatedAtUTC >= req.Payload.Payload.AfterDateUpdate));

        if (req.Payload.Payload.StatusesFilter is not null && req.Payload.Payload.StatusesFilter.Length != 0)
            q = q.Where(x => req.Payload.Payload.StatusesFilter.Any(y => y == x.StatusDocument));

        IOrderedQueryable<OrderDocumentModelDB> oq = req.SortingDirection == VerticalDirectionsEnum.Up
           ? q.OrderBy(x => x.CreatedAtUTC)
           : q.OrderByDescending(x => x.CreatedAtUTC);

        IQueryable<OrderDocumentModelDB> pq = oq
            .Skip(req.PageNum * req.PageSize)
            .Take(req.PageSize);

        Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<OrderDocumentModelDB, NomenclatureModelDB?> inc_query = pq
            .Include(x => x.Organization)
            .Include(x => x.AddressesTabs!)
            .ThenInclude(x => x.AddressOrganization)
            .Include(x => x.AddressesTabs!)
            .ThenInclude(x => x.Rows!)
            .ThenInclude(x => x.Offer!)
            .ThenInclude(x => x.Nomenclature);

        return new()
        {
            Response = new()
            {
                PageNum = req.PageNum,
                PageSize = req.PageSize,
                SortingDirection = req.SortingDirection,
                SortBy = req.SortBy,
                TotalRowsCount = await q.CountAsync(),
                Response = req.Payload.Payload.IncludeExternalData ? [.. await inc_query.ToArrayAsync()] : [.. await pq.ToArrayAsync()]
            },
        };
    }

    /// <inheritdoc/>
    public async Task<TResponseModel<int>> RowForOrderUpdate(RowOfOrderDocumentModelDB req)
    {
        TResponseModel<int> res = new() { Response = 0 };
        if (req.Quantity == 0)
        {
            res.AddError($"Количество не может быть нулевым");
            return res;
        }

        using CommerceContext context = await commerceDbFactory.CreateDbContextAsync();
        IQueryable<OrderRowsQueryRecord> queryDocumentDb = from r in context.RowsOfOrdersDocuments
                                                           join d in context.OrdersDocuments on r.OrderDocumentId equals d.Id
                                                           join t in context.TabsAddressesForOrders.Where(x => x.Id == req.AddressForOrderTabId) on r.AddressForOrderTabId equals t.Id
                                                           join o in context.Offers on r.OfferId equals o.Id
                                                           join g in context.Nomenclatures on r.NomenclatureId equals g.Id
                                                           select new OrderRowsQueryRecord(d, t, r, o, g);

        var commDataDb = await queryDocumentDb
            .Select(x => new
            {
                x.TabAddress.WarehouseId,
                x.Document.StatusDocument,
                OfferName = x.Offer.Name,
                GoodsName = x.Goods.Name,
            })
            .FirstAsync();

        bool conflict = await context.RowsOfOrdersDocuments
            .AnyAsync(x => x.Id != req.Id && x.AddressForOrderTabId == req.AddressForOrderTabId && x.OfferId == req.OfferId);

        if (commDataDb.WarehouseId == 0)
            res.AddError($"В документе не указан склад: обновление невозможно");

        if (conflict)
            res.AddError($"В документе уже существует этот оффер. Установите ему требуемое количество.");

        if (!res.Success())
            return res;

        RowOfOrderDocumentModelDB? rowDb = req.Id > 0
           ? await context.RowsOfOrdersDocuments.FirstAsync(x => x.Id == req.Id)
           : null;

        using IDbContextTransaction transaction = context.Database.BeginTransaction(System.Data.IsolationLevel.Serializable);

        List<LockOffersAvailabilityModelDB> lockers = [new()
        {
            LockerName = nameof(OfferAvailabilityModelDB),
            LockerId = req.OfferId,
            RubricId = commDataDb.WarehouseId,
        }];

        if (rowDb is not null && rowDb.OfferId != req.OfferId)
        {
            lockers.Add(new()
            {
                LockerName = nameof(OfferAvailabilityModelDB),
                LockerId = rowDb.OfferId,
                RubricId = commDataDb.WarehouseId,
            });
        }

        string msg;
        try
        {
            await context.AddRangeAsync(lockers);
            await context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            msg = $"Не удалось выполнить команду: ";
            loggerRepo.LogError(ex, $"{msg}{JsonConvert.SerializeObject(req, Formatting.Indented, GlobalStaticConstants.JsonSerializerSettings)}");
            res.AddError($"{msg}{ex.Message}");
            return res;
        }
        int[] _offersIds = [.. lockers.Select(x => x.LockerId)];
        OfferAvailabilityModelDB[] regsOfferAv = await context
            .OffersAvailability
            .Where(x => _offersIds.Any(y => y == x.OfferId))
            .Include(x => x.Offer)
            .ToArrayAsync();

        OfferAvailabilityModelDB? regOfferAv = regsOfferAv.FirstOrDefault(x => x.OfferId == req.OfferId && x.WarehouseId == commDataDb.WarehouseId);
        if (regOfferAv is null && commDataDb.StatusDocument != StatusesDocumentsEnum.Canceled)
        {
            regOfferAv = new()
            {
                OfferId = req.OfferId,
                NomenclatureId = req.NomenclatureId,
                WarehouseId = commDataDb.WarehouseId,
            };
            await context.AddAsync(regOfferAv);
        }

        OfferAvailabilityModelDB? regOfferAvStorno = null;
        if (rowDb is not null && rowDb.OfferId != req.OfferId)
        {
            regOfferAvStorno = regsOfferAv.FirstOrDefault(x => x.OfferId == rowDb.OfferId && x.WarehouseId == commDataDb.WarehouseId);

            if (regOfferAvStorno is null)
            {
                regOfferAvStorno = new()
                {
                    OfferId = rowDb.OfferId,
                    NomenclatureId = rowDb.NomenclatureId,
                    WarehouseId = commDataDb.WarehouseId,
                };
                await context.AddAsync(regOfferAvStorno);
            }
        }

        DateTime dtu = DateTime.UtcNow;
        await context.OrdersDocuments
                .Where(x => x.Id == req.OrderDocumentId)
                .ExecuteUpdateAsync(set => set
                .SetProperty(p => p.Version, Guid.NewGuid())
                .SetProperty(p => p.LastAtUpdatedUTC, dtu));

        if (req.Id < 1)
        {
            if (regOfferAv is not null && commDataDb.StatusDocument != StatusesDocumentsEnum.Canceled)
            {
                if (regOfferAv.Quantity < req.Quantity)
                    res.AddError($"Количество '{regOfferAv.Offer?.GetName()}' недостаточно: [{regOfferAv.Quantity}] < [{req.Quantity}]");
                else
                {
                    regOfferAv.Quantity -= req.Quantity;
                    context.OffersAvailability.Update(regOfferAv);
                }
            }
            else if (regOfferAv is null && commDataDb.StatusDocument != StatusesDocumentsEnum.Canceled)
                res.AddError($"Остаток ['{commDataDb.OfferName}' - '{commDataDb.GoodsName}'] отсутствует");

            if (regOfferAvStorno is not null)
            {
                regOfferAvStorno.Quantity += req.Quantity;
                if (regOfferAvStorno.Id > 0)
                    context.Update(regOfferAvStorno);
            }

            req.Version = Guid.NewGuid();
            await context.AddAsync(req);
            await context.SaveChangesAsync();
            res.AddSuccess("Товар добавлен к заказу");
            res.Response = req.Id;
        }
        else
        {
            if (rowDb!.Version != req.Version)
            {
                await transaction.RollbackAsync();
                msg = "Строка документа была уже кем-то изменена";
                loggerRepo.LogError($"{msg}: {JsonConvert.SerializeObject(req, Formatting.Indented, GlobalStaticConstants.JsonSerializerSettings)}");
                res.AddError($"{msg}. Обновите документ (F5), что бы получить актуальные данные");
                return res;
            }

            decimal _delta = req.Quantity - rowDb.Quantity;
            if (_delta == 0)
                res.AddInfo("Количество не изменилось");
            else if (regOfferAv is not null && commDataDb.StatusDocument != StatusesDocumentsEnum.Canceled)
            {
                regOfferAv.Quantity += _delta;
                if (regOfferAv.Id > 0)
                    context.Update(regOfferAv);

                if (regOfferAvStorno is not null)
                {
                    regOfferAvStorno.Quantity -= _delta;
                    context.Update(regOfferAvStorno);
                }
            }

            res.Response = await context.RowsOfOrdersDocuments
              .Where(x => x.Id == req.Id)
              .ExecuteUpdateAsync(set => set
              .SetProperty(p => p.Quantity, req.Quantity)
              .SetProperty(p => p.Amount, req.Amount)
              .SetProperty(p => p.Version, Guid.NewGuid()));
        }

        if (!res.Success())
        {
            await transaction.RollbackAsync();
            return res;
        }

        context.RemoveRange(lockers);
        await context.SaveChangesAsync();
        await transaction.CommitAsync();
        res.AddSuccess($"Обновление `строки документа-заказа` выполнено");
        return res;
    }

    /// <inheritdoc/>
    public async Task<TResponseModel<bool>> RowsForOrderDelete(int[] req)
    {
        string msg;
        req = [.. req.Distinct()];
        TResponseModel<bool> res = new() { Response = req.Any(x => x > 0) };
        if (!res.Response)
        {
            res.AddError("Пустой запрос");
            return res;
        }

        using CommerceContext context = await commerceDbFactory.CreateDbContextAsync();
        IQueryable<RowOfOrderDocumentModelDB> mainQuery = context.RowsOfOrdersDocuments.Where(x => req.Any(y => y == x.Id));
        IQueryable<RowOrderDocumentRecord> q = from r in mainQuery
                                               join d in context.OrdersDocuments on r.OrderDocumentId equals d.Id
                                               join t in context.TabsAddressesForOrders on r.AddressForOrderTabId equals t.Id
                                               select new RowOrderDocumentRecord(
                                                   d.Id,
                                                   d.StatusDocument,
                                                   r.OfferId,
                                                   r.NomenclatureId,
                                                   r.Quantity,
                                                   t.WarehouseId
                                               );
        RowOrderDocumentRecord[] _allOffersOfDocuments = await q
           .ToArrayAsync();

        if (_allOffersOfDocuments.Length == 0)
        {
            res.AddError($"Данные документа не найдены");
            return res;
        }

        DateTime dtu = DateTime.UtcNow;
        LockOffersAvailabilityModelDB[] offersLocked = _allOffersOfDocuments
            .DistinctBy(x => new { x.OfferId, x.WarehouseId })
            .Select(x => new LockOffersAvailabilityModelDB()
            {
                LockerName = nameof(OfferAvailabilityModelDB),
                LockerId = x.OfferId,
                RubricId = x.WarehouseId
            })
            .ToArray();

        using IDbContextTransaction transaction = context.Database.BeginTransaction(IsolationLevel.Serializable);

        try
        {
            await context.AddRangeAsync(offersLocked);
            await context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            msg = "Не удалось выполнить команду блокировки БД: ";
            res.AddError($"{msg}{ex.Message}");
            loggerRepo.LogError(ex, $"{msg}{JsonConvert.SerializeObject(req, Formatting.Indented, GlobalStaticConstants.JsonSerializerSettings)}");
            return res;
        }

        int[] _offersIds = [.. _allOffersOfDocuments.Select(x => x.OfferId).Distinct()];

        List<OfferAvailabilityModelDB> registersOffersDb = await context.OffersAvailability
           .Where(x => _offersIds.Any(y => y == x.OfferId))
           .ToListAsync();

        int[] documents_ids = [.. _allOffersOfDocuments.Select(x => x.DocumentId).Distinct()];
        await context.OrdersDocuments.Where(x => documents_ids.Any(y => y == x.Id)).ExecuteUpdateAsync(set => set.SetProperty(p => p.Version, Guid.NewGuid()).SetProperty(p => p.LastAtUpdatedUTC, DateTime.UtcNow));

        foreach (var rowEl in _allOffersOfDocuments.Where(x => x.DocumentStatus != StatusesDocumentsEnum.Canceled))
        {
            OfferAvailabilityModelDB? offerRegister = registersOffersDb.FirstOrDefault(x => x.OfferId == rowEl.OfferId && x.WarehouseId == rowEl.WarehouseId);
            if (offerRegister is not null)
            {
                offerRegister.Quantity += rowEl.Quantity;
                context.Update(offerRegister);
            }
            else
            {
                offerRegister = new()
                {
                    WarehouseId = rowEl.WarehouseId,
                    NomenclatureId = rowEl.GoodsId,
                    OfferId = rowEl.OfferId,
                    Quantity = +rowEl.Quantity,
                };
                await context.OffersAvailability.AddAsync(offerRegister);
                registersOffersDb.Add(offerRegister);
            }
        }

        if (offersLocked.Length != 0)
            context.RemoveRange(offersLocked);

        await context.SaveChangesAsync();
        res.Response = await context.RowsOfOrdersDocuments.Where(x => req.Any(y => y == x.Id)).ExecuteDeleteAsync() != 0;

        await transaction.CommitAsync();
        res.AddSuccess("Команда удаления выполнена");
        return res;
    }

    /// <inheritdoc/>
    public async Task<TResponseModel<int>> OrderUpdate(OrderDocumentModelDB req)
    {
        TResponseModel<int> res = new() { Response = 0 };
        ValidateReportModel ck = GlobalTools.ValidateObject(req);
        if (!ck.IsValid)
        {
            res.Messages.InjectException(ck.ValidationResults);
            return res;
        }

        req.Name = req.Name.Trim();

        TResponseModel<UserInfoModel[]?> actor = await webTransmissionRepo.GetUsersIdentity([req.AuthorIdentityUserId]);
        if (!actor.Success() || actor.Response is null || actor.Response.Length == 0)
        {
            res.AddRangeMessages(actor.Messages);
            return res;
        }

        string msg, waMsg;
        DateTime dtu = DateTime.UtcNow;
        req.LastAtUpdatedUTC = dtu;

        OfferModelDB?[] allOffersReq = req.AddressesTabs!
            .SelectMany(x => x.Rows!)
            .Select(x => x.Offer)
            .DistinctBy(x => x!.Id)
            .ToArray();

        allOffersReq = GlobalTools.CreateDeepCopy(allOffersReq)!;

        List<Task> tasks;
        using CommerceContext context = await commerceDbFactory.CreateDbContextAsync();
        if (req.Id < 1)
        {
            if (req.AddressesTabs is null || req.AddressesTabs.Count == 0)
            {
                res.AddError($"В заказе отсутствуют адреса доставки");
                return res;
            }

            req.AddressesTabs.ForEach(x =>
            {
                if (x.Rows is null || x.Rows.Count == 0)
                    res.AddError($"Для адреса доставки '{x.AddressOrganization?.Name}' не указана номенклатура");
                else if (x.Rows.Any(x => x.Quantity < 1))
                    res.AddError($"В адресе доставки '{x.AddressOrganization?.Name}' есть номенклатура без количества");
                else if (x.Rows.Count != x.Rows.GroupBy(x => x.OfferId).Count())
                    res.AddError($"В адресе доставки '{x.AddressOrganization?.Name}' ошибка в таблице товаров: оффер встречается более одного раза");

                if (x.WarehouseId < 1)
                    res.AddError($"В адресе доставки '{x.AddressOrganization?.Name}' не корректный склад #{x.WarehouseId}");
            });
            if (!res.Success())
                return res;

            int[] rubricsIds = [.. req.AddressesTabs.Select(x => x.WarehouseId).Distinct()];
            TResponseModel<List<RubricIssueHelpdeskModelDB>?> getRubrics = await hdRepo.RubricsGet(rubricsIds);
            if (!getRubrics.Success())
            {
                res.AddRangeMessages(getRubrics.Messages);
                return res;
            }

            if (getRubrics.Response is null || getRubrics.Response.Count != rubricsIds.Length)
            {
                res.AddError($"Некоторые склады (rubric`s) не найдены");
                return res;
            }

            req.Id = 0;
            req.CreatedAtUTC = dtu;
            req.LastAtUpdatedUTC = dtu;
            req.Version = Guid.NewGuid();
            req.StatusDocument = StatusesDocumentsEnum.Created;

            var _offersOfDocument = req.AddressesTabs
                           .SelectMany(x => x.Rows!.Select(y => new { x.WarehouseId, Row = y }))
                           .ToArray();

            using IDbContextTransaction transaction = context.Database.BeginTransaction(System.Data.IsolationLevel.Serializable);

            LockOffersAvailabilityModelDB[] offersLocked = _offersOfDocument.Select(x => new LockOffersAvailabilityModelDB()
            {
                LockerName = nameof(OfferAvailabilityModelDB),
                LockerId = x.Row.OfferId,
                RubricId = x.WarehouseId
            }).ToArray();

            try
            {
                await context.AddRangeAsync(offersLocked);
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                msg = "Не удалось выполнить команду блокировки БД: ";
                loggerRepo.LogError(ex, $"{msg}{JsonConvert.SerializeObject(req, Formatting.Indented, GlobalStaticConstants.JsonSerializerSettings)}");
                res.AddError($"{msg}{ex.Message}");
                return res;
            }

            int[] _offersIds = [.. allOffersReq.Select(x => x!.Id)];
            List<OfferAvailabilityModelDB> registersOffersDb = default!;
            TResponseModel<int?> res_RubricIssueForCreateOrder = default!;
            TResponseModel<string?>? CommerceNewOrderSubjectNotification = null, CommerceNewOrderBodyNotification = null, CommerceNewOrderBodyNotificationTelegram = null;

            tasks = [
                Task.Run(async () => { CommerceNewOrderSubjectNotification = await StorageTransmissionRepo.ReadParameter<string?>(GlobalStaticConstants.CloudStorageMetadata.CommerceNewOrderSubjectNotification); }),
                Task.Run(async () => { CommerceNewOrderBodyNotification = await StorageTransmissionRepo.ReadParameter<string?>(GlobalStaticConstants.CloudStorageMetadata.CommerceNewOrderBodyNotification); }),
                Task.Run(async () => { CommerceNewOrderBodyNotificationTelegram = await StorageTransmissionRepo.ReadParameter<string?>(GlobalStaticConstants.CloudStorageMetadata.CommerceNewOrderBodyNotificationTelegram); }),
                Task.Run(async () => { registersOffersDb = await context.OffersAvailability.Where(x => _offersIds.Any(y => y == x.OfferId)).ToListAsync();}),
                Task.Run(async () => { res_RubricIssueForCreateOrder = await StorageTransmissionRepo.ReadParameter<int?>(GlobalStaticConstants.CloudStorageMetadata.RubricIssueForCreateOrder);})];

            if (string.IsNullOrWhiteSpace(_webConf.ClearBaseUri))
            {
                tasks.Add(Task.Run(async () =>
                {
                    if (string.IsNullOrWhiteSpace(_webConf.ClearBaseUri))
                    {
                        TResponseModel<TelegramBotConfigModel?> wc = await webTransmissionRepo.GetWebConfig();
                        _webConf.BaseUri = wc.Response?.ClearBaseUri;
                    }
                }));
                TResponseModel<TelegramBotConfigModel?> wc = await webTransmissionRepo.GetWebConfig();
                _webConf.BaseUri = wc.Response?.ClearBaseUri;
            }

            await Task.WhenAll(tasks);
            tasks.Clear();

            req.AddressesTabs.ForEach(tabAddr =>
            {
                tabAddr.Rows!.ForEach(rowDoc =>
                {
                    OfferAvailabilityModelDB? rowReg = registersOffersDb.FirstOrDefault(x => x.OfferId == rowDoc.OfferId && x.WarehouseId == tabAddr.WarehouseId);
                    OfferModelDB offerInfo = allOffersReq.First(x => x?.Id == rowDoc.OfferId)!;

                    if (rowReg is null)
                        res.AddError($"'{offerInfo.Name}' (склад: `{getRubrics.Response.First(x => x.Id == tabAddr.WarehouseId).Name}`) нет в наличии");
                    else if (rowReg.Quantity < rowDoc.Quantity)
                        res.AddError($"'{offerInfo.Name}' (склад: `{getRubrics.Response.First(x => x.Id == tabAddr.WarehouseId).Name}`) не достаточно. Текущий остаток: {rowReg.Quantity}");
                });
            });
            if (!res.Success())
            {
                await transaction.RollbackAsync();
                return res;
            }

            req.PrepareForSave();
            req.CreatedAtUTC = dtu;
            try
            {
                await context.AddAsync(req);
                await context.SaveChangesAsync();
                res.Response = req.Id;

                foreach (TabAddressForOrderModelDb tabAddr in req.AddressesTabs)
                {
                    foreach (RowOfOrderDocumentModelDB rowDoc in tabAddr.Rows!)
                    {
                        OfferAvailabilityModelDB rowReg = registersOffersDb.First(x => x.OfferId == rowDoc.OfferId && x.WarehouseId == tabAddr.WarehouseId);
                        OfferModelDB offerInfo = allOffersReq.First(x => x?.Id == rowDoc.OfferId)!;
                        rowReg.Quantity -= rowDoc.Quantity;
                        context.Update(rowReg);
                    }
                }

                TAuthRequestModel<UniversalUpdateRequestModel> issue_new = new()
                {
                    SenderActionUserId = req.AuthorIdentityUserId,
                    Payload = new()
                    {
                        Name = req.Name,
                        ParentId = res_RubricIssueForCreateOrder.Response,
                        Description = $"Новый заказ.\n{req.Information}".Trim(),
                    },
                };

                TResponseModel<int> issue = await hdRepo.IssueCreateOrUpdate(issue_new);
                if (!issue.Success())
                {
                    await transaction.RollbackAsync();
                    res.Messages.AddRange(issue.Messages);
                    return res;
                }

                req.HelpdeskId = issue.Response;
                context.Update(req);

                string subject_email = "Создан новый заказ";
                DateTime _dt = DateTime.UtcNow.GetCustomTime();
                string _dtAsString = $"{_dt.ToString("d", cultureInfo)} {_dt.ToString("t", cultureInfo)}";
                string _about_order = $"'{req.Name}' {_dtAsString}";

                if (CommerceNewOrderSubjectNotification?.Success() == true && !string.IsNullOrWhiteSpace(CommerceNewOrderSubjectNotification.Response))
                    subject_email = CommerceNewOrderSubjectNotification.Response;

                subject_email = IHelpdeskService.ReplaceTags(req.Name, _dt, issue.Response, StatusesDocumentsEnum.Created, subject_email, _webConf.ClearBaseUri, _about_order);
                res.AddSuccess(subject_email);
                msg = $"<p>Заказ <b>'{issue_new.Payload.Name}' от [{_dtAsString}]</b> успешно создан.</p>" +
                        $"<p>/<a href='{_webConf.ClearBaseUri}'>{_webConf.ClearBaseUri}</a>/</p>";
                string msg_for_tg = msg.Replace("<p>", "").Replace("</p>", "");

                waMsg = $"Заказ '{issue_new.Payload.Name}' от [{_dtAsString}] успешно создан.\n{_webConf.ClearBaseUri}";

                if (CommerceNewOrderBodyNotification?.Success() == true && !string.IsNullOrWhiteSpace(CommerceNewOrderBodyNotification.Response))
                    msg = CommerceNewOrderBodyNotification.Response;
                msg = IHelpdeskService.ReplaceTags(req.Name, _dt, issue.Response, StatusesDocumentsEnum.Created, msg, _webConf.ClearBaseUri, _about_order);

                if (CommerceNewOrderBodyNotificationTelegram?.Success() == true && !string.IsNullOrWhiteSpace(CommerceNewOrderBodyNotificationTelegram.Response))
                    msg_for_tg = CommerceNewOrderBodyNotificationTelegram.Response;
                msg_for_tg = IHelpdeskService.ReplaceTags(req.Name, _dt, issue.Response, StatusesDocumentsEnum.Created, msg_for_tg, _webConf.ClearBaseUri, _about_order);

                tasks = [webTransmissionRepo.SendEmail(new() { Email = actor.Response[0].Email!, Subject = subject_email, TextMessage = msg }, false)];

                if (actor.Response[0].TelegramId.HasValue)
                    tasks.Add(tgRepo.SendTextMessageTelegram(new() { Message = msg_for_tg, UserTelegramId = actor.Response[0].TelegramId!.Value }, false));

                if (!string.IsNullOrWhiteSpace(actor.Response[0].PhoneNumber) && GlobalTools.IsPhoneNumber(actor.Response[0].PhoneNumber!))
                {
                    tasks.Add(Task.Run(async () =>
                    {
                        TResponseModel<string?> CommerceNewOrderBodyNotificationWhatsapp = await StorageTransmissionRepo.ReadParameter<string?>(GlobalStaticConstants.CloudStorageMetadata.CommerceNewOrderBodyNotificationWhatsapp);
                        if (CommerceNewOrderBodyNotificationWhatsapp.Success() && !string.IsNullOrWhiteSpace(CommerceNewOrderBodyNotificationWhatsapp.Response))
                            waMsg = CommerceNewOrderBodyNotificationWhatsapp.Response;

                        await tgRepo.SendWappiMessage(new() { Number = actor.Response[0].PhoneNumber!, Text = IHelpdeskService.ReplaceTags(req.Name, _dt, issue.Response, StatusesDocumentsEnum.Created, waMsg, _webConf.ClearBaseUri, _about_order, true) }, false);
                    }));
                }

                loggerRepo.LogInformation(msg_for_tg);
                context.RemoveRange(offersLocked);
                tasks.Add(context.SaveChangesAsync());
                await Task.WhenAll(tasks);
                await transaction.CommitAsync();
                return res;
            }
            catch (Exception ex)
            {
                loggerRepo.LogError(ex, $"Не удалось создать заявку-заказ: {JsonConvert.SerializeObject(req, GlobalStaticConstants.JsonSerializerSettings)}");
                res.Messages.InjectException(ex);
                await transaction.RollbackAsync();
                return res;
            }
        }

        OrderDocumentModelDB order_document = await context.OrdersDocuments.FirstAsync(x => x.Id == req.Id);
        if (order_document.Version != req.Version)
        {
            msg = "Документ был кем-то изменён пока был открытым";
            loggerRepo.LogError($"{msg}: {JsonConvert.SerializeObject(req, Formatting.Indented, GlobalStaticConstants.JsonSerializerSettings)}");
            res.AddError($"{msg}. Обновите сначала документ (F5)");
            return res;
        }

        if (order_document.Name == req.Name && order_document.Description == req.Description)
        {
            res.AddInfo($"Документ #{req.Id} не требует обновления");
            return res;
        }

        res.Response = await context.OrdersDocuments
            .Where(x => x.Id == req.Id)
            .ExecuteUpdateAsync(set => set
            .SetProperty(p => p.Name, req.Name)
            .SetProperty(p => p.Description, req.Description)
            .SetProperty(p => p.Version, Guid.NewGuid())
            .SetProperty(p => p.LastAtUpdatedUTC, dtu));

        res.AddSuccess($"Обновление `документа-заказа` выполнено");
        return res;
    }

    /// <inheritdoc/>
    public async Task<TResponseModel<bool>> StatusesOrdersChangeByHelpdeskDocumentId(StatusOrderChangeRequestModel req)
    {
        string msg;
        using CommerceContext context = await commerceDbFactory.CreateDbContextAsync();
        OrderDocumentModelDB[] ordersDb = await context
            .OrdersDocuments
            .Where(x => x.HelpdeskId == req.DocumentId && x.StatusDocument != req.Step)
            .Include(x => x.AddressesTabs!)
            .ThenInclude(x => x.Rows)
            .ToArrayAsync();

        TResponseModel<bool> res = new();

        if (ordersDb.Length == 0)
        {
            msg = "Изменение не требуется (документы для обновления отсутствуют)";
            loggerRepo.LogInformation($"{msg}: {JsonConvert.SerializeObject(req, Formatting.Indented, GlobalStaticConstants.JsonSerializerSettings)}");
            res.AddInfo($"{msg}. Перед редактированием обновите страницу (F5), что бы загрузить актуальную версию объекта");
            return res;
        }

        List<WarehouseRowDocumentRecord> _allOffersOfDocuments = ordersDb.SelectMany(x => x.AddressesTabs!)
                           .SelectMany(x => x.Rows!.Select(y => new WarehouseRowDocumentRecord(x.WarehouseId, y)))
                           .ToList();

        if (_allOffersOfDocuments.Count == 0)
        {
            res.Response = await context
                    .OrdersDocuments
                    .Where(x => x.HelpdeskId == req.DocumentId)
                    .ExecuteUpdateAsync(set => set.SetProperty(p => p.LastAtUpdatedUTC, DateTime.UtcNow)) != 0;

            res.AddSuccess("Запрос смены статуса заказа выполнен (строки в документе отсутствуют)");
            return res;
        }

        LockOffersAvailabilityModelDB[] offersLocked = _allOffersOfDocuments
            .DistinctBy(x => new { x.WarehouseId, x.Row.OfferId })
            .Select(x => new LockOffersAvailabilityModelDB()
            {
                LockerName = nameof(OfferAvailabilityModelDB),
                LockerId = x.Row.OfferId,
                RubricId = x.WarehouseId
            })
            .ToArray();

        using IDbContextTransaction transaction = context.Database.BeginTransaction(System.Data.IsolationLevel.Serializable);
        try
        {
            await context.AddRangeAsync(offersLocked);
            await context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            msg = $"Не удалось выполнить команду блокировки БД: ";
            loggerRepo.LogError(ex, $"{msg}{JsonConvert.SerializeObject(req, Formatting.Indented, GlobalStaticConstants.JsonSerializerSettings)}");
            res.AddError($"{msg}{ex.Message}");
            return res;
        }

        int[] _offersIds = [.. _allOffersOfDocuments.Select(x => x.Row.OfferId).Distinct()];
        List<OfferAvailabilityModelDB> registersOffersDb = await context.OffersAvailability
           .Where(x => _offersIds.Any(y => y == x.OfferId))
           .Include(x => x.Offer)
           .ToListAsync();

        _allOffersOfDocuments.ForEach(async offerEl =>
        {
            int _i = registersOffersDb.FindIndex(y => y.WarehouseId == offerEl.WarehouseId && y.OfferId == offerEl.Row.OfferId);

            if (req.Step == StatusesDocumentsEnum.Canceled)
            {
                if (_i < 0)
                {
                    OfferAvailabilityModelDB _newReg = new()
                    {
                        WarehouseId = offerEl.WarehouseId,
                        NomenclatureId = offerEl.Row.NomenclatureId,
                        OfferId = offerEl.Row.OfferId,
                        Quantity = offerEl.Row.Quantity,
                    };
                    registersOffersDb.Add(_newReg);
                    await context.AddAsync(_newReg);
                }
                else
                    registersOffersDb[_i].Quantity += offerEl.Row.Quantity;
            }
            else
            {
                if (_i < 0)
                    res.AddError($"Отсутствуют остатки [{offerEl.Row.Offer?.Name}] - списание {{{offerEl.Row.Quantity}}} невозможно");
                else if (registersOffersDb[_i].Quantity < offerEl.Row.Quantity)
                    res.AddError($"Недостаточно остатков [{offerEl.Row.Offer?.Name}] - списание {{{offerEl.Row.Quantity}}} отклонено");
                else
                    registersOffersDb[_i].Quantity -= offerEl.Row.Quantity;
            }
        });

        if (!res.Success())
        {
            await transaction.RollbackAsync();
            msg = $"Отказ изменения статуса: не достаточно остатков!";
            loggerRepo.LogError($"{JsonConvert.SerializeObject(req, Formatting.Indented, GlobalStaticConstants.JsonSerializerSettings)}\n{JsonConvert.SerializeObject(res, Formatting.Indented, GlobalStaticConstants.JsonSerializerSettings)}");
            res.AddError(msg);
            return res;
        }

        context.UpdateRange(registersOffersDb.Where(x => x.Id > 0));
        if (offersLocked.Length != 0)
            context.RemoveRange(offersLocked);

        await context.SaveChangesAsync();
        res.Response = await context
                            .OrdersDocuments
                            .Where(x => x.HelpdeskId == req.DocumentId)
                            .ExecuteUpdateAsync(set => set
                            .SetProperty(p => p.StatusDocument, req.Step)
                            .SetProperty(p => p.LastAtUpdatedUTC, DateTime.UtcNow)
                            .SetProperty(p => p.Version, Guid.NewGuid())) != 0;

        await transaction.CommitAsync();
        res.AddSuccess("Запрос смены статуса заказа выполнен успешно");
        return res;
    }

    /// <inheritdoc/>
    public async Task<TResponseModel<FileAttachModel>> GetOrderReportFile(TAuthRequestModel<int> req)
    {
        using CommerceContext context = await commerceDbFactory.CreateDbContextAsync();

        TResponseModel<UserInfoModel[]?> rest = default!;
        TResponseModel<OrderDocumentModelDB[]> orderData = default!;
        List<Task> _taskList = [
            Task.Run(async () => { rest = await webTransmissionRepo.GetUsersIdentity([req.SenderActionUserId]); }),
            Task.Run(async () => { orderData = await OrdersRead([req.Payload]); })];

        await Task.WhenAll(_taskList);

        if (!rest.Success() || rest.Response is null || rest.Response.Length != 1)
            return new() { Messages = rest.Messages };

        TResponseModel<FileAttachModel> res = new();
        if (!orderData.Success() || orderData.Response is null || orderData.Response.Length != 1)
        {
            res.AddRangeMessages(orderData.Messages);
            return res;
        }

        OrderDocumentModelDB orderDb = orderData.Response[0];
        UserInfoModel actor = rest.Response[0];
        bool allowed = actor.IsAdmin || orderDb.AuthorIdentityUserId == actor.UserId || actor.UserId == GlobalStaticConstants.Roles.System;
        if (!allowed && orderDb.HelpdeskId.HasValue && orderDb.HelpdeskId.Value > 0)
        {
            TResponseModel<IssueHelpdeskModelDB[]> issueData = await hdRepo.IssuesRead(new TAuthRequestModel<IssuesReadRequestModel>()
            {
                SenderActionUserId = req.SenderActionUserId,
                Payload = new()
                {
                    IssuesIds = [orderDb.HelpdeskId.Value],
                    IncludeSubscribersOnly = true,
                }
            });
            if (!issueData.Success() || issueData.Response is null || issueData.Response.Length != 1)
            {
                res.AddRangeMessages(issueData.Messages);
                return res;
            }
            IssueHelpdeskModelDB issueDb = issueData.Response[0];
            if (actor.UserId != issueDb.AuthorIdentityUserId && actor.UserId != issueDb.ExecutorIdentityUserId && !issueDb.Subscribers!.Any(s => s.UserId == actor.UserId))
            {
                res.AddError("У вас не доступа к этому документу");
                return res;
            }
        }
        else if (!allowed)
        {
            res.AddError("У вас не доступа к этому документу");
            return res;
        }

        string docName = $"Заказ {orderDb.Name} от {orderDb.CreatedAtUTC.GetHumanDateTime()}";

        try
        {
            res.Response = new()
            {
                Data = SaveOrderAsExcel(orderDb),
                ContentType = GlobalTools.ContentTypes.First(x => x.Value.Contains("xlsx")).Key,
                Name = $"{docName.Replace(":", "-").Replace(" ", "_")}.xlsx",
            };
        }
        catch (Exception ex)
        {
            loggerRepo.LogError(ex, $"Ошибка создания Excel документа: {docName}");
            div wrapDiv = new();
            wrapDiv.AddDomNode(new p(docName));

            orderDb.AddressesTabs!.ForEach(aNode =>
            {
                div addressDiv = new();
                addressDiv.AddDomNode(new p($"Адрес: `{aNode.AddressOrganization?.Name}`"));

                table my_table = new() { css_style = "border: 1px solid black; width: 100%; border-collapse: collapse;" };
                my_table.THead.AddColumn("Наименование").AddColumn("Цена").AddColumn("Кол-во").AddColumn("Сумма");

                aNode.Rows?.ForEach(dr =>
                {
                    my_table.TBody.AddRow([dr.Offer!.GetName(), dr.Offer.Price.ToString(), dr.Quantity.ToString(), dr.Amount.ToString()]);
                });
                addressDiv.AddDomNode(my_table);
                addressDiv.AddDomNode(new p($"Итого: {aNode.Rows!.Sum(x => x.Amount)}") { css_style = "float: right;" });
                wrapDiv.AddDomNode(addressDiv);
            });

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            string test_s = $"<style>table, th, td {{border: 1px solid black;border-collapse: collapse;}}</style>{wrapDiv.GetHTML()}";

            using MemoryStream ms = new();
            StreamWriter writer = new(ms);
            writer.Write(test_s);
            writer.Flush();
            ms.Position = 0;

            res.Response = new()
            {
                Data = ms.ToArray(),
                ContentType = GlobalTools.ContentTypes.First(x => x.Value.Contains("html")).Key,
                Name = $"{docName.Replace(":", "-").Replace(" ", "_")}.html",
            };
        }

        return res;
    }

    /// <inheritdoc/>
    public async Task<FileAttachModel> GetFullPriceFile()
    {
        string docName = $"Прайс на {DateTime.Now.GetHumanDateTime()}";
        using CommerceContext context = await commerceDbFactory.CreateDbContextAsync();
        List<OfferModelDB> offersAll = await context.Offers
            .Include(x => x.Nomenclature)
            .Include(x => x.Registers)
            .ToListAsync();

        if (offersAll.Count == 0)
        {
            loggerRepo.LogWarning($"Пустой прайс: {docName}");
            return new()
            {
                Data = [],
                ContentType = GlobalTools.ContentTypes.First(x => x.Value.Contains("html")).Key,
                Name = $"{docName.Replace(":", "-").Replace(" ", "_")}.html",
            };
        }

        int[] rubricsIds = offersAll.SelectMany(x => x.Registers!).Select(x => x.WarehouseId).Distinct().ToArray();
        TResponseModel<List<RubricIssueHelpdeskModelDB>?> rubricsDb = await hdRepo.RubricsGet(rubricsIds);
        List<IGrouping<NomenclatureModelDB?, OfferModelDB>> gof = offersAll.GroupBy(x => x.Nomenclature).Where(x => x.Any(y => y.Registers!.Any(z => z.Quantity > 0))).ToList();
        try
        {
            return new()
            {
                Data = ExportPrice(gof, rubricsDb.Response),
                ContentType = GlobalTools.ContentTypes.First(x => x.Value.Contains("xlsx")).Key,
                Name = $"{docName.Replace(":", "-").Replace(" ", "_")}.xlsx",
            };
        }
        catch (Exception ex)
        {
            loggerRepo.LogError(ex, $"Ошибка создания Excel документа: {docName}");
            return new()
            {
                Data = [],
                ContentType = GlobalTools.ContentTypes.First(x => x.Value.Contains("html")).Key,
                Name = $"{docName.Replace(":", "-").Replace(" ", "_")}.html",
            };
        }
    }

    /// <inheritdoc/>
    public async Task<TResponseModel<int>> NomenclatureUpdate(NomenclatureModelDB nom)
    {
        nom.Name = nom.Name.Trim();
        loggerRepo.LogInformation($"call `{GetType().Name}`: {JsonConvert.SerializeObject(nom, GlobalStaticConstants.JsonSerializerSettings)}");
        TResponseModel<int> res = new() { Response = 0 };
        using CommerceContext context = await commerceDbFactory.CreateDbContextAsync();
        string msg, about = $"'{nom.Name}' /{nom.BaseUnit}";
        NomenclatureModelDB? nomenclature_db = await context.Nomenclatures.FirstOrDefaultAsync(x => x.Name == nom.Name && x.BaseUnit == nom.BaseUnit && x.Id != nom.Id);

        if (nomenclature_db is not null)
        {
            msg = $"Ошибка создания Номенклатуры {about}. Такой объект уже существует #{nomenclature_db.Id}. Требуется уникальное сочетание имени и единицы измерения";
            loggerRepo.LogWarning(msg);
            res.AddError(msg);
            return res;
        }
        DateTime dtu = DateTime.UtcNow;
        nom.LastAtUpdatedUTC = dtu;

        if (nom.Id < 1)
        {
            nom.Id = 0;
            nom.CreatedAtUTC = dtu;
            nomenclature_db = nom;
            nom.SortIndex = await context.Nomenclatures.MaxAsync(x => x.SortIndex) + 1;

            await context.AddAsync(nomenclature_db);
            await context.SaveChangesAsync();
            msg = $"Номенклатура {about} создана #{nomenclature_db.Id}";
            loggerRepo.LogInformation(msg);
            res.AddSuccess(msg);
            res.Response = nomenclature_db.Id;
            return res;
        }

        res.Response = await context.Nomenclatures
            .Where(x => x.Id == nom.Id)
            .ExecuteUpdateAsync(set => set
            .SetProperty(p => p.Name, nom.Name)
            .SetProperty(p => p.NormalizedNameUpper, nom.Name.ToUpper().Trim())
            .SetProperty(p => p.Description, nom.Description)
            .SetProperty(p => p.BaseUnit, nom.BaseUnit)
            .SetProperty(p => p.IsDisabled, nom.IsDisabled)
            .SetProperty(p => p.ContextName, nom.ContextName)
            .SetProperty(p => p.ProjectId, nom.ProjectId)
            .SetProperty(p => p.LastAtUpdatedUTC, dtu));

        msg = $"Обновление номенклатуры {about} выполнено";
        loggerRepo.LogInformation(msg);
        res.AddSuccess(msg);
        return res;
    }

    static byte[] SaveOrderAsExcel(OrderDocumentModelDB orderDb)
    {
        string docName = $"Заказ {orderDb.Name} от {orderDb.CreatedAtUTC.GetHumanDateTime()}";
        using MemoryStream XLSStream = new();


        WorkbookPart? wBookPart = null;

        using SpreadsheetDocument spreadsheetDoc = SpreadsheetDocument.Create(XLSStream, SpreadsheetDocumentType.Workbook);

        wBookPart = spreadsheetDoc.AddWorkbookPart();
        wBookPart.Workbook = new Workbook();
        uint sheetId = 1;
        WorkbookPart workbookPart = spreadsheetDoc.WorkbookPart ?? spreadsheetDoc.AddWorkbookPart();

        WorkbookStylesPart wbsp = workbookPart.AddNewPart<WorkbookStylesPart>();

        wbsp.Stylesheet = GenerateExcelStyleSheet();
        wbsp.Stylesheet.Save();

        workbookPart.Workbook.Sheets = new Sheets();

        Sheets sheets = workbookPart.Workbook.GetFirstChild<Sheets>() ?? workbookPart.Workbook.AppendChild(new Sheets());

        foreach (var table in orderDb.AddressesTabs!)
        {
            WorksheetPart wSheetPart = wBookPart.AddNewPart<WorksheetPart>();
            Sheet sheet = new() { Id = workbookPart.GetIdOfPart(wSheetPart), SheetId = sheetId, Name = table.AddressOrganization?.Name };
            sheets.Append(sheet);

            SheetData sheetData = new();
            wSheetPart.Worksheet = new Worksheet(sheetData);

            Columns lstColumns = wSheetPart.Worksheet.GetFirstChild<Columns>()!;
            bool needToInsertColumns = false;
            if (lstColumns == null)
            {
                lstColumns = new Columns();
                needToInsertColumns = true;
            }

            lstColumns.Append(new Column() { Min = 1, Max = 1, Width = 100, CustomWidth = true, BestFit = true, });
            lstColumns.Append(new Column() { Min = 2, Max = 2, Width = 8, CustomWidth = true, BestFit = true, });
            lstColumns.Append(new Column() { Min = 3, Max = 3, Width = 8, CustomWidth = true, BestFit = true, });
            lstColumns.Append(new Column() { Min = 4, Max = 4, Width = 15, CustomWidth = true, BestFit = true, });

            if (needToInsertColumns)
                wSheetPart.Worksheet.InsertAt(lstColumns, 0);

            Row topRow = new() { RowIndex = 2 };
            InsertExcelCell(topRow, 1, $"Адрес доставки: {table.AddressOrganization?.Address}", CellValues.String, 0);
            sheetData!.Append(topRow);

            Row headerRow = new() { RowIndex = 4 };
            InsertExcelCell(headerRow, 1, "Наименование", CellValues.String, 1);
            InsertExcelCell(headerRow, 2, "Цена", CellValues.String, 1);
            InsertExcelCell(headerRow, 3, "Кол-во", CellValues.String, 1);
            InsertExcelCell(headerRow, 4, "Сумма", CellValues.String, 1);
            sheetData.AppendChild(headerRow);

            uint row_index = 5;
            foreach (RowOfOrderDocumentModelDB dr in table.Rows!)
            {
                Row row = new() { RowIndex = row_index++ };
                InsertExcelCell(row, 1, dr.Offer!.GetName(), CellValues.String, 0);
                InsertExcelCell(row, 2, dr.Offer.Price.ToString(), CellValues.String, 0);
                InsertExcelCell(row, 3, dr.Quantity.ToString(), CellValues.String, 0);
                InsertExcelCell(row, 4, dr.Amount.ToString(), CellValues.String, 0);
                sheetData.Append(row);
            }
            Row btRow = new() { RowIndex = row_index++ };
            InsertExcelCell(btRow, 1, "", CellValues.String, 0);
            InsertExcelCell(btRow, 2, "", CellValues.String, 0);
            InsertExcelCell(btRow, 3, "Итого:", CellValues.String, 0);
            InsertExcelCell(btRow, 4, table.Rows!.Sum(x => x.Amount).ToString(), CellValues.String, 0);
            sheetData.Append(btRow);
            sheetId++;
        }

        workbookPart.Workbook.Save();
        spreadsheetDoc.Save();

        XLSStream.Position = 0;
        return XLSStream.ToArray();
    }

    static byte[] ExportPrice(List<IGrouping<NomenclatureModelDB?, OfferModelDB>> sourceTable, List<RubricIssueHelpdeskModelDB>? rubricsDb)
    {
        WorkbookPart? wBookPart = null;
        using MemoryStream XLSStream = new();
        using SpreadsheetDocument spreadsheetDoc = SpreadsheetDocument.Create(XLSStream, SpreadsheetDocumentType.Workbook);

        wBookPart = spreadsheetDoc.AddWorkbookPart();
        wBookPart.Workbook = new Workbook();
        uint sheetId = 1;
        WorkbookPart workbookPart = spreadsheetDoc.WorkbookPart ?? spreadsheetDoc.AddWorkbookPart();

        WorkbookStylesPart wbsp = workbookPart.AddNewPart<WorkbookStylesPart>();

        wbsp.Stylesheet = GenerateExcelStyleSheet();
        wbsp.Stylesheet.Save();

        workbookPart.Workbook.Sheets = new Sheets();

        Sheets sheets = workbookPart.Workbook.GetFirstChild<Sheets>() ?? workbookPart.Workbook.AppendChild(new Sheets());

        foreach (IGrouping<NomenclatureModelDB?, OfferModelDB> table in sourceTable)
        {
            WorksheetPart wSheetPart = wBookPart.AddNewPart<WorksheetPart>();
            Sheet sheet = new() { Id = workbookPart.GetIdOfPart(wSheetPart), SheetId = sheetId, Name = table.Key?.Name };
            sheets.Append(sheet);

            SheetData sheetData = new();
            wSheetPart.Worksheet = new Worksheet(sheetData);

            Columns lstColumns = wSheetPart.Worksheet.GetFirstChild<Columns>()!;
            bool needToInsertColumns = false;
            if (lstColumns == null)
            {
                lstColumns = new Columns();
                needToInsertColumns = true;
            }

            lstColumns.Append(new Column() { Min = 1, Max = 1, Width = 100, CustomWidth = true, BestFit = true, });
            lstColumns.Append(new Column() { Min = 2, Max = 2, Width = 8, CustomWidth = true, BestFit = true, });
            lstColumns.Append(new Column() { Min = 3, Max = 3, Width = 8, CustomWidth = true, BestFit = true, });
            lstColumns.Append(new Column() { Min = 4, Max = 4, Width = 15, CustomWidth = true, BestFit = true, });

            if (needToInsertColumns)
                wSheetPart.Worksheet.InsertAt(lstColumns, 0);

            Row topRow = new() { RowIndex = 2 };
            InsertExcelCell(topRow, 1, $"Дата формирования: {DateTime.Now.GetHumanDateTime()}", CellValues.String, 0);
            sheetData!.Append(topRow);

            Row headerRow = new() { RowIndex = 4 };
            InsertExcelCell(headerRow, 1, "Наименование", CellValues.String, 1);
            InsertExcelCell(headerRow, 2, "Цена", CellValues.String, 1);
            InsertExcelCell(headerRow, 3, "Ед.изм.", CellValues.String, 1);
            InsertExcelCell(headerRow, 4, "Остаток/Склад", CellValues.String, 1);
            sheetData.AppendChild(headerRow);

            uint row_index = 5;
            foreach (OfferModelDB dr in table)
            {
                foreach (IGrouping<int, OfferAvailabilityModelDB> nodeG in dr.Registers!.GroupBy(x => x.WarehouseId))
                {
                    Row row = new() { RowIndex = row_index++ };
                    sheetData.AppendChild(row);
                    InsertExcelCell(row, 1, dr!.GetName(), CellValues.String, 0);
                    InsertExcelCell(row, 2, dr.Price.ToString(), CellValues.String, 0);
                    InsertExcelCell(row, 3, dr.OfferUnit.DescriptionInfo(), CellValues.String, 0);
                    InsertExcelCell(row, 4, $"{nodeG.Sum(x => x.Quantity)} /{rubricsDb?.FirstOrDefault(r => r.Id == nodeG.Key)?.Name}", CellValues.String, 0);
                };
            }
            Row btRow = new() { RowIndex = row_index++ };
            InsertExcelCell(btRow, 1, "", CellValues.String, 0);
            InsertExcelCell(btRow, 2, "", CellValues.String, 0);
            InsertExcelCell(btRow, 3, "Итого:", CellValues.String, 0);
            InsertExcelCell(btRow, 4, table!.Sum(x => x.Registers!.Sum(y => y.Quantity)).ToString(), CellValues.String, 0);
            sheetData.Append(btRow);
            sheetId++;
        }

        workbookPart.Workbook.Save();
        spreadsheetDoc.Save();
        XLSStream.Position = 0;

        return XLSStream.ToArray();
    }

    static Stylesheet GenerateExcelStyleSheet()
    {
        return new Stylesheet(
            new Fonts(
                new Font(                                                               // Стиль под номером 0 - Шрифт по умолчанию.
                    new FontSize() { Val = 11 },
                    new Color() { Rgb = new HexBinaryValue() { Value = "000000" } },
                    new FontName() { Val = "Calibri" }),
                new Font(                                                               // Стиль под номером 1 - Жирный шрифт Times New Roman.
                    new Bold(),
                    new FontSize() { Val = 11 },
                    new Color() { Rgb = new HexBinaryValue() { Value = "000000" } },
                    new FontName() { Val = "Times New Roman" }),
                new Font(                                                               // Стиль под номером 2 - Обычный шрифт Times New Roman.
                    new FontSize() { Val = 11 },
                    new Color() { Rgb = new HexBinaryValue() { Value = "000000" } },
                    new FontName() { Val = "Times New Roman" }),
                new Font(                                                               // Стиль под номером 3 - Шрифт Times New Roman размером 14.
                    new FontSize() { Val = 14 },
                    new Color() { Rgb = new HexBinaryValue() { Value = "000000" } },
        new FontName() { Val = "Times New Roman" })
        ),
        new Fills(
                new Fill(                                                           // Стиль под номером 0 - Заполнение ячейки по умолчанию.
        new PatternFill() { PatternType = PatternValues.None }),
                new Fill(                                                           // Стиль под номером 1 - Заполнение ячейки серым цветом
                    new PatternFill(
                        new ForegroundColor() { Rgb = new HexBinaryValue() { Value = "FFAAAAAA" } }
        )
                    { PatternType = PatternValues.Solid }),
        new Fill(                                                           // Стиль под номером 2 - Заполнение ячейки красным.
                    new PatternFill(
                        new ForegroundColor() { Rgb = new HexBinaryValue() { Value = "FFEFEFEF" } }
                    )
                    { PatternType = PatternValues.Solid })
            )
            ,
            new Borders(
                new Border(                                                         // Стиль под номером 0 - Грани.
                    new LeftBorder(),
                    new RightBorder(),
                    new TopBorder(),
                    new BottomBorder(),
                    new DiagonalBorder()),
                new Border(                                                         // Стиль под номером 1 - Грани
                    new LeftBorder(
                        new Color() { Auto = true }
                    )
                    { Style = BorderStyleValues.Medium },
                    new RightBorder(
                        new Color() { Indexed = (UInt32Value)64U }
                    )
                    { Style = BorderStyleValues.Medium },
                    new TopBorder(
                        new Color() { Auto = true }
                    )
                    { Style = BorderStyleValues.Medium },
                    new BottomBorder(
                        new Color() { Indexed = (UInt32Value)64U }
                    )
                    { Style = BorderStyleValues.Medium },
                    new DiagonalBorder()),
                new Border(                                                         // Стиль под номером 2 - Грани.
                    new LeftBorder(
                        new Color() { Auto = true }
                    )
                    { Style = BorderStyleValues.Thin },
                    new RightBorder(
                        new Color() { Indexed = (UInt32Value)64U }
                    )
                    { Style = BorderStyleValues.Thin },
                    new TopBorder(
                        new Color() { Auto = true }
                    )
                    { Style = BorderStyleValues.Thin },
                    new BottomBorder(
                        new Color() { Indexed = (UInt32Value)64U }
                    )
                    { Style = BorderStyleValues.Thin },
                    new DiagonalBorder())
            ),
            new CellFormats(
                new CellFormat() { FontId = 0, FillId = 0, BorderId = 0 },                          // Стиль под номером 0 - The default cell style.  (по умолчанию)
                new CellFormat(new Alignment() { Horizontal = HorizontalAlignmentValues.Center, Vertical = VerticalAlignmentValues.Center, WrapText = true }) { FontId = 1, FillId = 2, BorderId = 1, ApplyFont = true },       // Стиль под номером 1 - Bold 
                new CellFormat(new Alignment() { Horizontal = HorizontalAlignmentValues.Center, Vertical = VerticalAlignmentValues.Center, WrapText = true }) { FontId = 2, FillId = 0, BorderId = 2, ApplyFont = true },       // Стиль под номером 2 - REgular
                new CellFormat() { FontId = 3, FillId = 0, BorderId = 2, ApplyFont = true, NumberFormatId = 4 },       // Стиль под номером 3 - Times Roman
                new CellFormat() { FontId = 0, FillId = 2, BorderId = 0, ApplyFill = true },       // Стиль под номером 4 - Yellow Fill
                new CellFormat(                                                                   // Стиль под номером 5 - Alignment
                    new Alignment() { Horizontal = HorizontalAlignmentValues.Center, Vertical = VerticalAlignmentValues.Center }
                )
                { FontId = 0, FillId = 0, BorderId = 0, ApplyAlignment = true },
                new CellFormat() { FontId = 0, FillId = 0, BorderId = 1, ApplyBorder = true },      // Стиль под номером 6 - Border
                new CellFormat(new Alignment() { Horizontal = HorizontalAlignmentValues.Right, Vertical = VerticalAlignmentValues.Center, WrapText = true }) { FontId = 2, FillId = 0, BorderId = 2, ApplyFont = true, NumberFormatId = 4 }       // Стиль под номером 7 - Задает числовой формат полю.
            )
        );
    }

    static void InsertExcelCell(Row row, int cell_num, string val, CellValues type, uint styleIndex)
    {
        Cell? refCell = null;
        Cell newCell = new() { CellReference = cell_num.ToString() + ":" + row.RowIndex?.ToString(), StyleIndex = styleIndex };
        row.InsertBefore(newCell, refCell);

        newCell.CellValue = new CellValue(val);
        newCell.DataType = new EnumValue<CellValues>(type);

    }
}

record WarehouseDocumentRecord(int WarehouseId, bool IsDisabled);

record OrderRowsQueryRecord(OrderDocumentModelDB Document, TabAddressForOrderModelDb TabAddress, RowOfOrderDocumentModelDB Row, OfferModelDB Offer, NomenclatureModelDB Goods);

record WarehouseRowDocumentRecord(int WarehouseId, RowOfOrderDocumentModelDB Row);

record RowOrderDocumentRecord(int DocumentId, StatusesDocumentsEnum DocumentStatus, int OfferId, int GoodsId, decimal Quantity, int WarehouseId);