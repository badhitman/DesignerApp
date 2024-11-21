////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;
using System.Globalization;
using Newtonsoft.Json;
using SharedLib;
using DbcLib;
using Microsoft.EntityFrameworkCore.Storage;

namespace CommerceService;

/// <summary>
/// Commerce
/// </summary>
public class CommerceImplementService(
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

        Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<OrderDocumentModelDB, GoodsModelDB?> inc_query = q
            .Include(x => x.Organization)
            .Include(x => x.AddressesTabs!)
            .ThenInclude(x => x.AddressOrganization)
            .Include(x => x.AddressesTabs!)
            .ThenInclude(x => x.Rows!)
            .ThenInclude(x => x.Offer!)
            .ThenInclude(x => x.Goods);

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
            .ThenInclude(x => x.Rows!)
            .ThenInclude(x => x.Offer!)
            .ThenInclude(x => x.Goods)
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

        if (req.Payload.Payload.GoodsFilter.HasValue && req.Payload.Payload.GoodsFilter.Value != 0)
            q = q.Where(x => context.RowsOfOrdersDocuments.Any(y => y.OrderDocumentId == x.Id && y.GoodsId == req.Payload.Payload.GoodsFilter));

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

        Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<OrderDocumentModelDB, GoodsModelDB?> inc_query = pq
            .Include(x => x.Organization)
            .Include(x => x.AddressesTabs!)
            .ThenInclude(x => x.AddressOrganization)
            .Include(x => x.AddressesTabs!)
            .ThenInclude(x => x.Rows!)
            .ThenInclude(x => x.Offer!)
            .ThenInclude(x => x.Goods);

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

        if (await context.RowsOfOrdersDocuments.AnyAsync(x => x.Id != req.Id && x.OrderDocumentId == req.OrderDocumentId && x.OfferId == req.OfferId))
        {
            res.AddError($"В документе не указан склад: удаление невозможно");
            return res;
        }

        var queryDocumentDb = from r in context.RowsOfOrdersDocuments.Where(x => x.Id == req.Id)
                              join d in context.OrdersDocuments on r.OrderDocumentId equals d.Id
                              join t in context.TabsAddressesForOrders on r.OrderDocumentId equals t.Id
                              select new { Document = d, TabAddress = t, Row = r };

        StatusesDocumentsEnum orderDocStatus = await queryDocumentDb
            .Select(x => x.Document.StatusDocument)
            .FirstAsync();

        int _warehouseId = await context.TabsAddressesForOrders.Where(x => x.Id == req.OrderDocumentId).Select(x => x.WarehouseId).FirstAsync();
        bool conflict = await context.RowsOfOrdersDocuments.AnyAsync(x => x.Id != req.Id && x.AddressForOrderTabId == req.AddressForOrderTabId && x.OfferId == req.OfferId);

        List<Task> tasks = [];

        if (_warehouseId == 0)
            res.AddError($"В документе не указан склад: обновление невозможно");

        if (conflict)
            res.AddError($"В документе уже существует этот оффер. Установите ему требуемое количество.");

        if (!res.Success())
            return res;

        RowOfOrderDocumentModelDB? rowDb = req.Id > 0
           ? await context.RowsOfOrdersDocuments.FirstAsync(x => x.Id == req.Id)
           : null;

        using IDbContextTransaction transaction = context.Database.BeginTransaction(System.Data.IsolationLevel.Serializable);

        List<LockOffersAvailabilityModelDB> lokers = [new()
        {
            LockerName = nameof(OfferAvailabilityModelDB),
            LockerId = req.OfferId,
            RubricId = _warehouseId,
        }];

        if (rowDb is not null && rowDb.OfferId != req.OfferId)
        {
            lokers.Add(new()
            {
                LockerName = nameof(OfferAvailabilityModelDB),
                LockerId = rowDb.OfferId,
                RubricId = _warehouseId,
            });
        }

        string msg;
        try
        {
            await context.AddRangeAsync(lokers);
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

        OfferAvailabilityModelDB? regOfferAv = await context
            .OffersAvailability
            .FirstOrDefaultAsync(x => x.OfferId == req.OfferId && x.WarehouseId == _warehouseId);

        if (regOfferAv is null && orderDocStatus != StatusesDocumentsEnum.Canceled)
        {
            regOfferAv = new()
            {
                OfferId = req.OfferId,
                GoodsId = req.GoodsId,
                WarehouseId = _warehouseId,
            };

            tasks.Add(Task.Run(async () => await context.AddAsync(regOfferAv)));
        }

        OfferAvailabilityModelDB? regOfferAvStorno = null;
        if (rowDb is not null && rowDb.OfferId != req.OfferId)
        {
            regOfferAvStorno = new()
            {
                OfferId = rowDb.OfferId,
                GoodsId = rowDb.GoodsId,
                WarehouseId = _warehouseId,
            };
        }

        tasks.Add(queryDocumentDb.Select(x => x.Document).ExecuteUpdateAsync(set => set
                            .SetProperty(p => p.LastAtUpdatedUTC, DateTime.UtcNow)
                            .SetProperty(p => p.Version, Guid.NewGuid())));

        DateTime dtu = DateTime.UtcNow;
        await context.OrdersDocuments
                .Where(x => x.Id == req.OrderDocumentId)
                .ExecuteUpdateAsync(set => set.SetProperty(p => p.LastAtUpdatedUTC, dtu));

        if (req.Id < 1)
        {
            if (regOfferAv is not null && orderDocStatus != StatusesDocumentsEnum.Canceled)
                regOfferAv.Quantity -= req.Quantity;

            req.Version = Guid.NewGuid();
            await context.AddAsync(req);
            await context.SaveChangesAsync();
            res.AddSuccess("Товар добавлен к заказу");
            res.Response = req.Id;
            return res;
        }

        else
        {
            if (rowDb!.Version != req.Version)
            {
                await Task.WhenAll(tasks);
                await transaction.RollbackAsync();
                msg = "Строка документа была уже кем-то изменена";
                loggerRepo.LogError($"{msg}: {JsonConvert.SerializeObject(req, Formatting.Indented, GlobalStaticConstants.JsonSerializerSettings)}");
                res.AddError($"{msg}. Обновите документ (F5), что бы получить актуальные данные");
                return res;
            }

            int _delta = rowDb.Quantity - req.Quantity;
            if (_delta == 0)
                res.AddInfo("Количество не изменилось");
            else if (regOfferAv is not null && orderDocStatus != StatusesDocumentsEnum.Canceled)
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

            tasks.Add(Task.Run(async () => res.Response = await context.RowsOfOrdersDocuments
                       .Where(x => x.Id == req.Id)
                       .ExecuteUpdateAsync(set => set
                       .SetProperty(p => p.Quantity, req.Quantity)
                       .SetProperty(p => p.Amount, req.Amount)
                       .SetProperty(p => p.Version, Guid.NewGuid()))));
        }

        context.RemoveRange(lokers);
        tasks.Add(context.SaveChangesAsync());
        await Task.WhenAll(tasks);
        await context.SaveChangesAsync();
        await transaction.CommitAsync();
        res.AddSuccess($"Обновление `строки документа-заказа` выполнено");
        return res;
    }

    /// <inheritdoc/>
    public async Task<TResponseModel<bool>> RowsForOrderDelete(int[] req)
    {

        string msg;
        TResponseModel<bool> res = new() { Response = req.Any(x => x > 0) };
        if (!res.Response)
        {
            res.AddError("Пустой запрос");
            return res;
        }
        req = [.. req.Distinct()];

        using CommerceContext context = await commerceDbFactory.CreateDbContextAsync();

        IQueryable<RowOfOrderDocumentModelDB> mainQuery = context.RowsOfOrdersDocuments.Where(x => req.Any(y => y == x.Id));
        var q = from r in mainQuery
                join d in context.OrdersDocuments on r.OrderDocumentId equals d.Id
                join t in context.TabsAddressesForOrders on r.AddressForOrderTabId equals t.Id
                select new
                {
                    DocumentId = d.Id,
                    DocumentStatus = d.StatusDocument,
                    r.OfferId,
                    r.GoodsId,
                    r.Quantity,
                    t.WarehouseId,
                };
        var _allOffersOfDocuments = await q
           .ToArrayAsync();

        if (_allOffersOfDocuments.Length == 0)
        {
            res.AddError($"Данные документа не найдены");
            return res;
        }

        DateTime dtu = DateTime.UtcNow;

        LockOffersAvailabilityModelDB[] offersLocked = _allOffersOfDocuments
            .Select(x => new LockOffersAvailabilityModelDB()
            {
                LockerName = nameof(OfferAvailabilityModelDB),
                LockerId = x.OfferId,
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
            msg = "Не удалось выполнить команду блокировки БД: ";
            res.AddError($"{msg}{ex.Message}");
            loggerRepo.LogError(ex, $"{msg}{JsonConvert.SerializeObject(req, Formatting.Indented, GlobalStaticConstants.JsonSerializerSettings)}");
            return res;
        }

        int[] _offersIds = [.. _allOffersOfDocuments.Select(x => x.OfferId)];

        List<OfferAvailabilityModelDB> registersOffersDb = await context.OffersAvailability
           .Where(x => _offersIds.Any(y => y == x.OfferId))
           .ToListAsync();
        int[] documents_ids = [.. _allOffersOfDocuments.Select(x => x.DocumentId)];
        List<Task> tasks = [.. documents_ids.Select(doc_id => context.OrdersDocuments.Where(x => x.Id == doc_id).ExecuteUpdateAsync(set => set.SetProperty(p => p.Version, Guid.NewGuid()).SetProperty(p => p.LastAtUpdatedUTC, DateTime.UtcNow)))];

        foreach (var rowEl in _allOffersOfDocuments.Where(x => x.DocumentStatus != StatusesDocumentsEnum.Canceled))
        {
            OfferAvailabilityModelDB? offerRegister = registersOffersDb.FirstOrDefault(x => x.OfferId == rowEl.OfferId && x.WarehouseId == rowEl.WarehouseId);
            if (offerRegister is not null)
            {
                offerRegister.Quantity -= rowEl.Quantity;
                context.Update(offerRegister);
            }
            else
                tasks.Add(Task.Run(async () =>
                {
                    await context.OffersAvailability.AddAsync(new()
                    {
                        WarehouseId = rowEl.WarehouseId,
                        GoodsId = rowEl.GoodsId,
                        OfferId = rowEl.OfferId,
                        Quantity = -rowEl.Quantity,
                    });
                }));

        }

        if (offersLocked.Length != 0)
            context.RemoveRange(offersLocked);

        tasks.Add(context.SaveChangesAsync());
        tasks.Add(Task.Run(async () => { res.Response = await context.RowsOfOrdersDocuments.Where(x => req.Any(y => y == x.Id)).ExecuteDeleteAsync() != 0; }));

        await Task.WhenAll(tasks);
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

        using CommerceContext context = await commerceDbFactory.CreateDbContextAsync();
        string msg, waMsg;
        DateTime dtu = DateTime.UtcNow;
        req.LastAtUpdatedUTC = dtu;
        req.PrepareForSave();
        List<Task> tasks;
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

            req.CreatedAtUTC = dtu;
            req.LastAtUpdatedUTC = dtu;
            req.Id = 0;
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

            int[] _offersIds = [.. _offersOfDocument.Select(x => x.Row.OfferId)];
            List<OfferAvailabilityModelDB> registersOffersDb = default!;
            TResponseModel<int?> res_RubricIssueForCreateOrder = default!;

            tasks = [
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

            req.CreatedAtUTC = dtu;
            try
            {
                await context.AddAsync(req);
                await context.SaveChangesAsync();
                res.Response = req.Id;

                TAuthRequestModel<IssueUpdateRequestModel> issue_new = new()
                {
                    SenderActionUserId = req.AuthorIdentityUserId,
                    Payload = new()
                    {
                        Name = req.Name,
                        RubricId = res_RubricIssueForCreateOrder.Response,
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

                foreach (TabAddressForOrderModelDb tabAddr in req.AddressesTabs)
                {
                    foreach (RowOfOrderDocumentModelDB rowDoc in tabAddr.Rows!)
                    {
                        OfferAvailabilityModelDB? rowReg = registersOffersDb.FirstOrDefault(x => x.OfferId == rowDoc.OfferId && x.WarehouseId == tabAddr.WarehouseId);
                        if (rowReg is null)
                        {
                            tasks.Add(Task.Run(async () =>
                            {
                                await context.AddAsync(new OfferAvailabilityModelDB()
                                {
                                    WarehouseId = tabAddr.WarehouseId,
                                    GoodsId = rowDoc.GoodsId,
                                    OfferId = rowDoc.OfferId,
                                    Quantity = rowDoc.Quantity,
                                });
                            }));
                        }
                        else
                        {
                            rowReg.Quantity -= rowDoc.Quantity;
                            context.Update(rowReg);
                        }
                    }
                }
                await Task.WhenAll(tasks);
                tasks.Clear();
                await context.SaveChangesAsync();

                string subject_email = "Создан новый заказ";
                DateTime _dt = DateTime.UtcNow.GetCustomTime();
                string _dtAsString = $"{_dt.ToString("d", cultureInfo)} {_dt.ToString("t", cultureInfo)}";
                string _about_order = $"'{req.Name}' {_dtAsString}";
                TResponseModel<string?>? CommerceNewOrderSubjectNotification = null, CommerceNewOrderBodyNotification = null, CommerceNewOrderBodyNotificationTelegram = null;

                req.HelpdeskId = issue.Response;
                context.Update(req);

                tasks = [
                    context.SaveChangesAsync(),
                    Task.Run(async () => { CommerceNewOrderSubjectNotification = await StorageTransmissionRepo.ReadParameter<string?>(GlobalStaticConstants.CloudStorageMetadata.CommerceNewOrderSubjectNotification); }),
                    Task.Run(async () => { CommerceNewOrderBodyNotification = await StorageTransmissionRepo.ReadParameter<string?>(GlobalStaticConstants.CloudStorageMetadata.CommerceNewOrderBodyNotification); }),
                    Task.Run(async () => { CommerceNewOrderBodyNotificationTelegram = await StorageTransmissionRepo.ReadParameter<string?>(GlobalStaticConstants.CloudStorageMetadata.CommerceNewOrderBodyNotificationTelegram); })];

                await Task.WhenAll(tasks);
                tasks.Clear();

                if (CommerceNewOrderSubjectNotification?.Success() == true && !string.IsNullOrWhiteSpace(CommerceNewOrderSubjectNotification.Response))
                    subject_email = CommerceNewOrderSubjectNotification.Response;

                string ReplaceTags(string raw, bool clearMd = false)
                {
                    return raw.Replace(GlobalStaticConstants.OrderDocumentName, req.Name)
                    .Replace(GlobalStaticConstants.OrderDocumentDate, $"{_dtAsString}")
                    .Replace(GlobalStaticConstants.OrderStatusInfo, StatusesDocumentsEnum.Created.DescriptionInfo())
                    .Replace(GlobalStaticConstants.OrderLinkAddress, clearMd ? $"{_webConf.BaseUri}/issue-card/{req.HelpdeskId}" : $"<a href='{_webConf.BaseUri}/issue-card/{req.HelpdeskId}'>{_about_order}</a>")
                    .Replace(GlobalStaticConstants.HostAddress, clearMd ? _webConf.BaseUri : $"<a href='{_webConf.BaseUri}'>{_webConf.BaseUri}</a>");
                }

                subject_email = ReplaceTags(subject_email);
                res.AddSuccess(subject_email);
                msg = $"<p>Заказ <b>'{issue_new.Payload.Name}' от [{_dtAsString}]</b> успешно создан.</p>" +
                        $"<p>/<a href='{_webConf.ClearBaseUri}'>{_webConf.ClearBaseUri}</a>/</p>";
                string msg_for_tg = msg.Replace("<p>", "").Replace("</p>", "");

                waMsg = $"Заказ '{issue_new.Payload.Name}' от [{_dtAsString}] успешно создан.\n{_webConf.ClearBaseUri}";

                if (CommerceNewOrderBodyNotification?.Success() == true && !string.IsNullOrWhiteSpace(CommerceNewOrderBodyNotification.Response))
                    msg = CommerceNewOrderBodyNotification.Response;
                msg = ReplaceTags(msg);

                if (CommerceNewOrderBodyNotificationTelegram?.Success() == true && !string.IsNullOrWhiteSpace(CommerceNewOrderBodyNotificationTelegram.Response))
                    msg_for_tg = CommerceNewOrderBodyNotificationTelegram.Response;
                msg_for_tg = ReplaceTags(msg_for_tg);

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

                        await tgRepo.SendWappiMessage(new() { Number = actor.Response[0].PhoneNumber!, Text = ReplaceTags(waMsg, true) }, false);
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
    public async Task<TResponseModel<bool>> StatusOrderChange(StatusOrderChangeRequestModel req)
    {
        string msg;
        using CommerceContext context = await commerceDbFactory.CreateDbContextAsync();
        OrderDocumentModelDB orderDb = await context
            .OrdersDocuments
            .Include(x => x.AddressesTabs!)
            .ThenInclude(x => x.Rows)
            .FirstAsync(x => x.Id == req.DocumentId);

        TResponseModel<bool> res = new();

        if (orderDb.StatusDocument == req.Step)
        {
            msg = $"Документу #{orderDb.Id} уже установлен нужный статус. Изменение не требуется";
            loggerRepo.LogInformation($"{msg}: {JsonConvert.SerializeObject(req, Formatting.Indented, GlobalStaticConstants.JsonSerializerSettings)}");
            res.AddInfo($"{msg}. Перед редактированием обновите страницу (F5), что бы загрузить актуальную версию объекта");
            return res;
        }

        if (orderDb.Version != req.VersionDocument)
        {
            msg = $"Документ #{orderDb.Id} был кем-то изменён (version concurent)";
            loggerRepo.LogError($"{msg}: {JsonConvert.SerializeObject(req, Formatting.Indented, GlobalStaticConstants.JsonSerializerSettings)}");
            res.AddError($"{msg}. Перед редактированием обновите страницу (F5), что бы загрузить актуальную версию объекта");
            return res;
        }

        var _allOffersOfDocuments = orderDb.AddressesTabs!
                           .SelectMany(x => x.Rows!.Select(y => new { x.WarehouseId, Row = y }))
                           .ToList();

        if (_allOffersOfDocuments.Count == 0)
        {
            res.Response = await context
                    .OrdersDocuments
                    .Where(x => x.HelpdeskId == req.DocumentId)
                    .ExecuteUpdateAsync(set => set.SetProperty(p => p.StatusDocument, req.Step)) != 0;

            res.AddSuccess("Запрос смены статуса заказа выполнен успешно");
        }

        LockOffersAvailabilityModelDB[] offersLocked = _allOffersOfDocuments
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

        int[] _offersIds = [.. _allOffersOfDocuments.Select(x => x.Row.OfferId)];
        List<OfferAvailabilityModelDB> registersOffersDb = await context.OffersAvailability
           .Where(x => _offersIds.Any(y => y == x.OfferId))
           .ToListAsync();

        _allOffersOfDocuments.ForEach(offerEl =>
        {
            int _i = registersOffersDb.FindIndex(y => y.WarehouseId == offerEl.WarehouseId && y.OfferId == offerEl.Row.OfferId);

            if (req.Step == StatusesDocumentsEnum.Canceled)
                registersOffersDb[_i].Quantity -= offerEl.Row.Quantity;
            else if (orderDb.StatusDocument == StatusesDocumentsEnum.Canceled)
                registersOffersDb[_i].Quantity += offerEl.Row.Quantity;
        });

        context.UpdateRange(registersOffersDb);

        List<Task> _tasks = [
            context.SaveChangesAsync(),
            Task.Run(async () => {
            res.Response = await context
                    .OrdersDocuments
                    .Where(x => x.HelpdeskId == req.DocumentId)
                    .ExecuteUpdateAsync(set => set
                    .SetProperty(p => p.StatusDocument, req.Step)
                    .SetProperty(p => p.LastAtUpdatedUTC, DateTime.UtcNow)
                    .SetProperty(p => p.Version, Guid.NewGuid())) != 0;
            }),
            ];

        await Task.WhenAll(_tasks);
        await transaction.CommitAsync();
        res.AddSuccess("Запрос смены статуса заказа выполнен успешно");
        return res;
    }


    /// <inheritdoc/>
    public async Task<TResponseModel<int>> WarehouseDocumentUpdate(WarehouseDocumentModelDB req)
    {
        TResponseModel<int> res = new() { Response = 0 };
        ValidateReportModel ck = GlobalTools.ValidateObject(req);
        if (!ck.IsValid)
        {
            res.Messages.InjectException(ck.ValidationResults);
            return res;
        }

        req.DeliveryDate = req.DeliveryDate.SetKindUtc();
        req.Name = req.Name.Trim();
        req.NormalizedUpperName = req.Name.ToUpper();

        using CommerceContext context = await commerceDbFactory.CreateDbContextAsync();
        string msg;
        DateTime dtu = DateTime.UtcNow;
        if (req.Id < 1)
        {
            req.Rows?.Clear();
            req.Id = 0;
            req.Version = Guid.NewGuid();
            req.CreatedAtUTC = dtu;
            req.LastAtUpdatedUTC = dtu;
            await context.AddAsync(req);
            await context.SaveChangesAsync();
            res.Response = req.Id;
            res.AddSuccess("Документ создан");
            return res;
        }

        WarehouseDocumentModelDB warehouseDocumentDb = await context
            .WarehouseDocuments
            .Include(x => x.Rows)
            .FirstAsync(x => x.Id == req.Id);

        if (warehouseDocumentDb.Version != req.Version)
        {
            msg = $"Документ #{warehouseDocumentDb.Id} был кем-то изменён (version concurent)";
            loggerRepo.LogError($"{msg}: {JsonConvert.SerializeObject(req, Formatting.Indented, GlobalStaticConstants.JsonSerializerSettings)}");
            res.AddError($"{msg}. Перед редактированием обновите страницу (F5), что бы загрузить актуальную версию объекта");
            return res;
        }

        LockOffersAvailabilityModelDB[] offersLocked = warehouseDocumentDb.Rows!.Count == 0
            ? []
            : warehouseDocumentDb.Rows.Select(x => new LockOffersAvailabilityModelDB() { LockerName = nameof(OfferAvailabilityModelDB), LockerId = x.OfferId, RubricId = req.WarehouseId }).ToArray();

        using IDbContextTransaction transaction = context.Database.BeginTransaction(System.Data.IsolationLevel.Serializable);
        if (offersLocked.Length != 0)
        {
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
        }

        List<Task> _tasks = [];
        int[] _offersIds = [.. warehouseDocumentDb.Rows.Select(x => x.OfferId).Distinct()];
        List<OfferAvailabilityModelDB> registersOffersDb = await context.OffersAvailability
            .Where(x => _offersIds.Any(y => y == x.OfferId))
            .ToListAsync();

        if (warehouseDocumentDb.IsDisabled != req.IsDisabled)
        {
            if (req.IsDisabled)
            {
                warehouseDocumentDb.Rows.ForEach(rowOfDocument =>
                {
                    OfferAvailabilityModelDB? registerOffer = registersOffersDb.FirstOrDefault(x => x.OfferId == rowOfDocument.OfferId && x.WarehouseId == warehouseDocumentDb.WarehouseId);
                    if (registerOffer is not null)
                        _tasks.Add(context.OffersAvailability.Where(y => true).ExecuteUpdateAsync(set => set.SetProperty(p => p.Quantity, registerOffer.Quantity - rowOfDocument.Quantity)));
                });
            }
            else
            {
                warehouseDocumentDb.Rows.ForEach(rowOfDocument =>
                {
                    OfferAvailabilityModelDB? registerOffer = registersOffersDb.FirstOrDefault(x => x.OfferId == rowOfDocument.OfferId && x.WarehouseId == req.WarehouseId);

                    if (registerOffer is not null)
                        _tasks.Add(context.OffersAvailability
                            .Where(y => y.Id == registerOffer.Id)
                            .ExecuteUpdateAsync(set => set.SetProperty(p => p.Quantity, registerOffer.Quantity + rowOfDocument.Quantity)));
                    else
                        _tasks.Add(Task.Run(async () =>
                        {
                            await context.OffersAvailability.AddAsync(new OfferAvailabilityModelDB()
                            {
                                WarehouseId = warehouseDocumentDb.WarehouseId,
                                GoodsId = rowOfDocument.GoodsId,
                                OfferId = rowOfDocument.OfferId,
                                Quantity = rowOfDocument.Quantity,
                            });
                        }));

                    if (warehouseDocumentDb.WarehouseId != req.WarehouseId)
                    {
                        registerOffer = registersOffersDb.FirstOrDefault(x => x.OfferId == rowOfDocument.OfferId && x.WarehouseId == warehouseDocumentDb.WarehouseId);

                        if (registerOffer is not null)
                            _tasks.Add(context.OffersAvailability.Where(y => y.Id == registerOffer.Id).ExecuteUpdateAsync(set => set.SetProperty(p => p.Quantity, registerOffer.Quantity - rowOfDocument.Quantity)));
                        else
                            _tasks.Add(Task.Run(async () =>
                            {
                                await context.OffersAvailability.AddAsync(new OfferAvailabilityModelDB()
                                {
                                    WarehouseId = warehouseDocumentDb.WarehouseId,
                                    GoodsId = rowOfDocument.GoodsId,
                                    OfferId = rowOfDocument.OfferId,
                                    Quantity = -rowOfDocument.Quantity,
                                });
                            }));
                    }
                });
            }
        }

        _tasks.Add(Task.Run(async () =>
        {
            res.Response = await context.WarehouseDocuments
                .Where(x => x.Id == req.Id)
                .ExecuteUpdateAsync(set => set
                .SetProperty(p => p.Name, req.Name)
                .SetProperty(p => p.Description, req.Description)
                .SetProperty(p => p.DeliveryDate, req.DeliveryDate)
                .SetProperty(p => p.IsDisabled, req.IsDisabled)
                .SetProperty(p => p.WarehouseId, req.WarehouseId)
                .SetProperty(p => p.Version, Guid.NewGuid())
                .SetProperty(p => p.LastAtUpdatedUTC, dtu));
        }));

        if (offersLocked.Length != 0)
            context.RemoveRange(offersLocked);

        res.AddSuccess("Складской документ обновлён");
        await Task.WhenAll(_tasks);
        await context.SaveChangesAsync();
        await transaction.CommitAsync();
        return res;
    }

    /// <inheritdoc/>
    public async Task<TResponseModel<bool>> RowsForWarehouseDocumentDelete(int[] req)
    {
        string msg;
        TResponseModel<bool> res = new() { Response = req.Any(x => x > 0) };
        if (!res.Response)
        {
            res.AddError("Пустой запрос");
            return res;
        }
        req = [.. req.Distinct()];
        using CommerceContext context = await commerceDbFactory.CreateDbContextAsync();
        IQueryable<RowOfWarehouseDocumentModelDB> mainQuery = context.RowsOfWarehouseDocuments.Where(x => req.Any(y => y == x.Id));
        var q = from r in mainQuery
                join d in context.WarehouseDocuments on r.WarehouseDocumentId equals d.Id
                select new
                {
                    DocumentId = d.Id,
                    d.IsDisabled,
                    d.WarehouseId,
                    r.OfferId,
                    r.GoodsId,
                    r.Quantity
                };

        var _allOffersOfDocuments = await q
           .ToArrayAsync();

        if (_allOffersOfDocuments.Length == 0)
        {
            res.AddWarning($"Данные не найдены. Метод удаления [{nameof(RowsForWarehouseDocumentDelete)}] не может выполнить команду.");
            return res;
        }
        LockOffersAvailabilityModelDB[] offersLocked = _allOffersOfDocuments
            .Select(x => new LockOffersAvailabilityModelDB()
            {
                LockerName = nameof(OfferAvailabilityModelDB),
                LockerId = x.OfferId,
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

        int[] _offersIds = [.. _allOffersOfDocuments.Select(x => x.OfferId)];
        List<OfferAvailabilityModelDB> registersOffersDb = await context.OffersAvailability
           .Where(x => _offersIds.Any(y => y == x.OfferId))
           .ToListAsync();

        int[] documents_ids = [.. _allOffersOfDocuments.Select(x => x.DocumentId).Distinct()];
        List<Task> tasks = [.. documents_ids.Select(doc_id => context.WarehouseDocuments.Where(x => x.Id == doc_id).ExecuteUpdateAsync(set => set.SetProperty(p => p.Version, Guid.NewGuid()).SetProperty(p => p.LastAtUpdatedUTC, DateTime.UtcNow)))];

        foreach (var rowEl in _allOffersOfDocuments.Where(x => !x.IsDisabled))
        {
            OfferAvailabilityModelDB? offerRegister = registersOffersDb.FirstOrDefault(x => x.OfferId == rowEl.OfferId && x.WarehouseId == rowEl.WarehouseId);
            if (offerRegister is not null)
            {
                offerRegister.Quantity -= rowEl.Quantity;
                context.Update(offerRegister);
            }
            else
                tasks.Add(Task.Run(async () =>
                {
                    await context.OffersAvailability.AddAsync(new()
                    {
                        WarehouseId = rowEl.WarehouseId,
                        GoodsId = rowEl.GoodsId,
                        OfferId = rowEl.OfferId,
                        Quantity = -rowEl.Quantity,
                    });
                }));
        }

        if (offersLocked.Length != 0)
            context.RemoveRange(offersLocked);

        tasks.Add(Task.Run(async () => res.Response = await context.RowsOfWarehouseDocuments.Where(x => req.Any(y => y == x.Id)).ExecuteDeleteAsync() != 0));

        await Task.WhenAll(tasks);
        await context.SaveChangesAsync();
        await transaction.CommitAsync();

        res.AddSuccess("Команда удаления выполнена");
        return res;
    }

    /// <inheritdoc/>
    public async Task<TResponseModel<int>> RowForWarehouseDocumentUpdate(RowOfWarehouseDocumentModelDB req)
    {
        string msg;
        TResponseModel<int> res = new() { Response = 0 };
        if (req.Quantity == 0)
        {
            res.AddError($"Количество не может быть нулевым");
            return res;
        }

        using CommerceContext context = await commerceDbFactory.CreateDbContextAsync();
        IQueryable<WarehouseDocumentModelDB> queryDocumentDb = from r in context.RowsOfWarehouseDocuments.Where(x => x.Id == req.Id)
                                                               join d in context.WarehouseDocuments on r.WarehouseDocumentId equals d.Id
                                                               select d;

        WarehouseDocumentRecord whDoc = await context.WarehouseDocuments.Where(x => x.Id == req.WarehouseDocumentId).Select(x => new WarehouseDocumentRecord(x.WarehouseId, x.IsDisabled)).FirstAsync();

        List<Task> tasks = [];
        if (whDoc.WarehouseId == 0)
        {
            msg = "В документе не указан склад";
            loggerRepo.LogError($"{msg}: {JsonConvert.SerializeObject(req, Formatting.Indented, GlobalStaticConstants.JsonSerializerSettings)}");
            res.AddError(msg);
            return res;
        }

        if (await context.RowsOfWarehouseDocuments.AnyAsync(x => x.Id != req.Id && x.OfferId == req.OfferId && x.WarehouseDocumentId == req.WarehouseDocumentId))
        {
            msg = "В документе уже существует этот офер. Установите ему требуемое количество";
            loggerRepo.LogError($"{msg}: {JsonConvert.SerializeObject(req, Formatting.Indented, GlobalStaticConstants.JsonSerializerSettings)}");
            res.AddError(msg);
            return res;
        }

        if (!res.Success())
            return res;

        RowOfWarehouseDocumentModelDB? rowDb = req.Id > 0
            ? await context.RowsOfWarehouseDocuments.FirstAsync(x => x.Id == req.Id)
            : null;

        using IDbContextTransaction transaction = context.Database.BeginTransaction(System.Data.IsolationLevel.Serializable);
        List<LockOffersAvailabilityModelDB> lokers = [new()
        {
            LockerName = nameof(OfferAvailabilityModelDB),
            LockerId = req.OfferId,
            RubricId = whDoc.WarehouseId,
        }];

        if (rowDb is not null && rowDb.OfferId != req.OfferId)
        {
            lokers.Add(new()
            {
                LockerName = nameof(OfferAvailabilityModelDB),
                LockerId = rowDb.OfferId,
                RubricId = whDoc.WarehouseId,
            });
        }

        try
        {
            await context.AddRangeAsync(lokers);
            await context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            msg = $"Не удалось выполнить команду: ";
            res.AddError($"{msg}{ex.Message}");
            loggerRepo.LogError(ex, $"{msg}{JsonConvert.SerializeObject(req, Formatting.Indented, GlobalStaticConstants.JsonSerializerSettings)}");
            return res;
        }

        OfferAvailabilityModelDB? regOfferAv = await context
            .OffersAvailability
            .FirstOrDefaultAsync(x => x.OfferId == req.OfferId && x.WarehouseId == whDoc.WarehouseId);

        if (regOfferAv is null && !whDoc.IsDisabled)
        {
            regOfferAv = new()
            {
                OfferId = req.OfferId,
                GoodsId = req.GoodsId,
                WarehouseId = whDoc.WarehouseId,
            };

            tasks.Add(Task.Run(async () => await context.AddAsync(regOfferAv)));
        }
        OfferAvailabilityModelDB? regOfferAvStorno = null;
        if (rowDb is not null && rowDb.OfferId != req.OfferId)
        {
            regOfferAvStorno = new()
            {
                OfferId = rowDb.OfferId,
                GoodsId = rowDb.GoodsId,
                WarehouseId = whDoc.WarehouseId,
            };
        }

        tasks.Add(queryDocumentDb.ExecuteUpdateAsync(set => set
            .SetProperty(p => p.LastAtUpdatedUTC, DateTime.UtcNow)
            .SetProperty(p => p.Version, Guid.NewGuid())));

        if (req.Id < 1)
        {
            if (regOfferAv is not null && !whDoc.IsDisabled)
                regOfferAv.Quantity += req.Quantity;

            req.Version = Guid.NewGuid();
            await context.AddAsync(req);
            await context.SaveChangesAsync();

            res.AddSuccess("Товар добавлен к документу");
            res.Response = req.Id;
        }
        else
        {
            if (rowDb!.Version != req.Version)
            {
                await Task.WhenAll(tasks);
                await transaction.RollbackAsync();
                msg = "Строка документа была уже кем-то изменена";
                loggerRepo.LogError($"{msg}: {JsonConvert.SerializeObject(req, Formatting.Indented, GlobalStaticConstants.JsonSerializerSettings)}");
                res.AddError($"{msg}. Обновите документ (F5), что бы получить актуальные данные");
                return res;
            }

            int _delta = req.Quantity - rowDb.Quantity;
            if (_delta == 0)
                res.AddInfo("Количество не изменилось");
            else if (regOfferAv is not null && !whDoc.IsDisabled)
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

            tasks.Add(Task.Run(async () => res.Response = await context.RowsOfWarehouseDocuments
                       .Where(x => x.Id == req.Id)
                       .ExecuteUpdateAsync(set => set
                       .SetProperty(p => p.Quantity, req.Quantity).SetProperty(p => p.Version, Guid.NewGuid()))));
        }
        context.RemoveRange(lokers);
        await Task.WhenAll(tasks);
        await context.SaveChangesAsync();
        await transaction.CommitAsync();

        res.AddSuccess($"Обновление `строки складского документа` выполнено");
        return res;
    }

    /// <inheritdoc/>
    public async Task<TResponseModel<WarehouseDocumentModelDB[]>> WarehouseDocumentsRead(int[] req)
    {
        TResponseModel<WarehouseDocumentModelDB[]> res = new();
        using CommerceContext context = await commerceDbFactory.CreateDbContextAsync();

        IQueryable<WarehouseDocumentModelDB> q = context
            .WarehouseDocuments
            .Where(x => req.Any(y => x.Id == y));

        res.Response = await q
            .Include(x => x.Rows!)
            .ThenInclude(x => x.Offer!)
            .ThenInclude(x => x.Goods)
            .ToArrayAsync();

        return res;
    }

    /// <inheritdoc/>
    public async Task<TResponseModel<TPaginationResponseModel<WarehouseDocumentModelDB>>> WarehouseDocumentsSelect(TPaginationRequestModel<WarehouseDocumentsSelectRequestModel> req)
    {
        if (req.PageSize < 10)
            req.PageSize = 10;

        using CommerceContext context = await commerceDbFactory.CreateDbContextAsync();

        IQueryable<WarehouseDocumentModelDB> q = context
            .WarehouseDocuments
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(req.Payload.SearchQuery))
            q = q.Where(x => x.NormalizedUpperName.Contains(req.Payload.SearchQuery.ToUpper()));

        if (req.Payload.OfferFilter.HasValue && req.Payload.OfferFilter.Value != 0)
            q = q.Where(x => context.RowsOfWarehouseDocuments.Any(y => y.WarehouseDocumentId == x.Id && y.OfferId == req.Payload.OfferFilter));

        if (req.Payload.GoodsFilter.HasValue && req.Payload.GoodsFilter.Value != 0)
            q = q.Where(x => context.RowsOfWarehouseDocuments.Any(y => y.WarehouseDocumentId == x.Id && y.GoodsId == req.Payload.GoodsFilter));

        if (req.Payload.AfterDateUpdate is not null)
            q = q.Where(x => x.LastAtUpdatedUTC >= req.Payload.AfterDateUpdate || (x.LastAtUpdatedUTC == DateTime.MinValue && x.CreatedAtUTC >= req.Payload.AfterDateUpdate));

        IOrderedQueryable<WarehouseDocumentModelDB> oq = req.SortingDirection == VerticalDirectionsEnum.Up
           ? q.OrderBy(x => x.CreatedAtUTC)
           : q.OrderByDescending(x => x.CreatedAtUTC);

        IQueryable<WarehouseDocumentModelDB> pq = oq
            .Skip(req.PageNum * req.PageSize)
            .Take(req.PageSize);

        Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<WarehouseDocumentModelDB, GoodsModelDB?> inc_query = pq
            .Include(x => x.Rows!)
            .ThenInclude(x => x.Offer!)
            .ThenInclude(x => x.Goods);

        return new()
        {
            Response = new()
            {
                PageNum = req.PageNum,
                PageSize = req.PageSize,
                SortingDirection = req.SortingDirection,
                SortBy = req.SortBy,
                TotalRowsCount = await q.CountAsync(),
                Response = req.Payload.IncludeExternalData ? [.. await inc_query.ToArrayAsync()] : [.. await pq.ToArrayAsync()]
            },
        };
    }

    /// <inheritdoc/>
    public async Task<TResponseModel<TPaginationResponseModel<OfferAvailabilityModelDB>>> RegistersSelect(TPaginationRequestModel<RegistersSelectRequestBaseModel> req)
    {
        if (req.PageSize < 10)
            req.PageSize = 10;

        using CommerceContext context = await commerceDbFactory.CreateDbContextAsync();

        IQueryable<OfferAvailabilityModelDB> q = context
            .OffersAvailability
            .AsQueryable();

        if (req.Payload.OfferFilter is not null && req.Payload.OfferFilter.Length != 0)
            q = q.Where(x => req.Payload.OfferFilter.Any(y => y == x.OfferId));

        if (req.Payload.GoodsFilter is not null && req.Payload.GoodsFilter.Length != 0)
            q = q.Where(x => req.Payload.GoodsFilter.Any(y => y == x.GoodsId));

        var exQuery = from offerAv in q
                      join oj in context.OffersGoods on offerAv.OfferId equals oj.Id
                      select new { offerAv, OfferGood = oj };

        IQueryable<OfferAvailabilityModelDB> oq = req.SortingDirection == VerticalDirectionsEnum.Up
           ? exQuery.OrderBy(x => x.OfferGood.Name).Select(x => x.offerAv)
           : exQuery.OrderByDescending(x => x.OfferGood.Name).Select(x => x.offerAv);

        IQueryable<OfferAvailabilityModelDB> pq = oq
            .Skip(req.PageNum * req.PageSize)
            .Take(req.PageSize);

        return new()
        {
            Response = new()
            {
                PageNum = req.PageNum,
                PageSize = req.PageSize,
                SortingDirection = req.SortingDirection,
                SortBy = req.SortBy,
                TotalRowsCount = await q.CountAsync(),
                Response = [.. await pq.Include(x => x.Goods).Include(x => x.Offer).ToArrayAsync()],
            },
        };
    }
}

internal record WarehouseDocumentRecord(int WarehouseId, bool IsDisabled);
