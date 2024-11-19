////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Net.Http.Json;
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
    static CultureInfo cultureInfo = new("ru-RU");

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
    public async Task<TResponseModel<int>> OrderUpdate(OrderDocumentModelDB req)
    {
        TResponseModel<int> res = new() { Response = 0 };
        if (req.AddressesTabs is null || req.AddressesTabs.Count == 0)
        {
            res.AddError($"В заказе отсутствуют адреса доставки");
            return res;
        }
        using CommerceContext context = await commerceDbFactory.CreateDbContextAsync();
        req.AddressesTabs.ForEach(async x =>
        {
            if (x.Rows is null || x.Rows.Count == 0)
                res.AddError($"Для адреса доставки '{x.AddressOrganization?.Name}' не указана номенклатура");
            else if (x.Rows.Any(x => x.Quantity < 1))
                res.AddError($"В адресе доставки '{x.AddressOrganization?.Name}' есть номенклатура без количества");
            else if (x.Rows.Count != x.Rows.GroupBy(x => x.OfferId).Count())
                res.AddError($"В адресе доставки '{x.AddressOrganization?.Name}' ошибка в таблице товаров: оффер встречается более одного раза");

            if (x.WarehouseId < 1 || (await hdRepo.RubricRead(x.WarehouseId)).Response?.Count != 1)
                res.AddError($"В адресе доставки '{x.AddressOrganization?.Name}' не корректный склад #{x.WarehouseId}");
        });
        if (!res.Success())
            return res;

        TResponseModel<UserInfoModel[]?> actor = await webTransmissionRepo.GetUsersIdentity([req.AuthorIdentityUserId]);
        if (!actor.Success() || actor.Response is null || actor.Response.Length == 0)
        {
            res.AddRangeMessages(actor.Messages);
            return res;
        }

        string msg, waMsg;
        DateTime dtu = DateTime.UtcNow;
        req.LastAtUpdatedUTC = dtu;
        req.PrepareForSave();
        List<Task> tasks;
        if (req.Id < 1)
        {
            req.Version = Guid.NewGuid();
            req.StatusDocument = StatusesDocumentsEnum.Created;
            using IDbContextTransaction transaction = context.Database.BeginTransaction(System.Data.IsolationLevel.Serializable);
            RowOfOrderDocumentModelDB[] _offersOfDocument = req.AddressesTabs
                            .SelectMany(x => x.Rows!)
                            .DistinctBy(x => x.OfferId)
                            .ToArray();

            LockOffersAvailabilityModelDB[] offersLocked = _offersOfDocument.Select(x => new LockOffersAvailabilityModelDB()
            {
                LockerName = nameof(OfferAvailabilityModelDB),
                LockerId = x.OfferId,
                RubricId = -1
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

            int[] _offersIds = [.. _offersOfDocument.Select(x => x.OfferId)];
            List<OfferAvailabilityModelDB> registersOffersDb = await context.OffersAvailability
                .Where(x => _offersIds.Any(y => y == x.OfferId))
                .ToListAsync();

            req.CreatedAtUTC = dtu;
            TResponseModel<int?> res_RubricIssueForCreateOrder = await StorageTransmissionRepo.ReadParameter<int?>(GlobalStaticConstants.CloudStorageMetadata.RubricIssueForCreateOrder);

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

                tasks = [];
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
                            rowReg.Quantity += rowDoc.Quantity;
                            context.Update(rowReg);
                        }
                    }
                }
                if (tasks.Count != 0)
                    await Task.WhenAll(tasks);

                await context.SaveChangesAsync();

                if (string.IsNullOrWhiteSpace(_webConf.ClearBaseUri))
                {
                    TResponseModel<TelegramBotConfigModel?> wc = await webTransmissionRepo.GetWebConfig();
                    _webConf.BaseUri = wc.Response?.ClearBaseUri;
                }

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

        res.AddSuccess($"Обновление `{GetType().Name}` выполнено");
        return res;
    }

    /// <inheritdoc/>
    public async Task<TResponseModel<bool>> RowsForOrderDelete(int[] req)
    {
        TResponseModel<bool> res = new() { Response = true };
        using CommerceContext context = await commerceDbFactory.CreateDbContextAsync();
        using IDbContextTransaction transaction = context.Database.BeginTransaction(System.Data.IsolationLevel.Serializable);
        DateTime dtu = DateTime.UtcNow;
        string msg;
        var q = from r in context.RowsOfOrdersDocuments.Where(x => req.Any(y => y == x.Id))
                join d in context.WarehouseDocuments on r.OrderDocumentId equals d.Id
                select new { DocumentId = d.Id, r.OfferId, r.GoodsId, r.Quantity, d.WarehouseId };

        var _offersOfDocument = await q
           .ToArrayAsync();

        if (_offersOfDocument.Length == 0)
        {
            res.AddError($"Данные документа не найдены");
            return res;
        }

        LockOffersAvailabilityModelDB[] offersLocked = _offersOfDocument.Length == 0
           ? []
           : _offersOfDocument.Select(x => new LockOffersAvailabilityModelDB() { LockerName = nameof(OfferAvailabilityModelDB), LockerId = x.OfferId, RubricId = x.WarehouseId }).ToArray();

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
                msg = "Не удалось выполнить команду блокировки БД: ";
                res.AddError($"{msg}{ex.Message}");
                loggerRepo.LogError(ex, $"{msg}{JsonConvert.SerializeObject(req, Formatting.Indented, GlobalStaticConstants.JsonSerializerSettings)}");
                return res;
            }
        }

        int[] _offersIds = [.. _offersOfDocument.Select(x => x.OfferId)];

        List<OfferAvailabilityModelDB> registersOffersDb = await context.OffersAvailability
           .Where(x => _offersIds.Any(y => y == x.OfferId))
           .ToListAsync();
        int[] documents_ids = [.. _offersOfDocument.Select(x => x.DocumentId)];
        List<Task> tasks = [.. documents_ids.Select(doc_id => context.OrdersDocuments.Where(x => x.Id == doc_id).ExecuteUpdateAsync(set => set.SetProperty(p => p.Version, Guid.NewGuid()).SetProperty(p => p.LastAtUpdatedUTC, DateTime.UtcNow)))];

        foreach (OfferAvailabilityModelDB rowEl in registersOffersDb)
        {
            var offerDoc = _offersOfDocument.First(x => x.OfferId == rowEl.OfferId);
            rowEl.Quantity -= offerDoc.Quantity;
        }
        context.UpdateRange(registersOffersDb);

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
    public async Task<TResponseModel<int>> RowForOrderUpdate(RowOfOrderDocumentModelDB req)
    {
        TResponseModel<int> res = new() { Response = 0 };
        using CommerceContext context = await commerceDbFactory.CreateDbContextAsync();
        int _rubricId = await context.TabsAddressesForOrders
            .Where(x => x.Id == req.OrderDocumentId)
            .Select(x => x.WarehouseId)
            .FirstAsync();

        if (_rubricId == 0)
        {
            res.AddError($"В документе не указан склад");
            return res;
        }

        using IDbContextTransaction transaction = context.Database.BeginTransaction(System.Data.IsolationLevel.Serializable);
        LockOffersAvailabilityModelDB locker = new()
        {
            LockerName = nameof(OfferAvailabilityModelDB),
            LockerId = req.OfferId,
            RubricId = _rubricId,
        };
        string msg;
        try
        {
            await context.AddAsync(locker);
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
            .FirstOrDefaultAsync(x => x.OfferId == req.OfferId && x.WarehouseId == _rubricId);

        if (regOfferAv is null)
        {
            regOfferAv = new()
            {
                OfferId = req.OfferId,
                GoodsId = req.GoodsId,
                WarehouseId = _rubricId,
            };

            await context.AddAsync(regOfferAv);
            await context.SaveChangesAsync();
        }

        List<Task> tasks = [];
        if (regOfferAv.GoodsId != req.GoodsId)
            tasks.Add(context.OffersAvailability.Where(x => x.Id == regOfferAv.Id).ExecuteUpdateAsync(set => set.SetProperty(p => p.GoodsId, req.GoodsId)));

        IQueryable<OrderDocumentModelDB> q = from r in context.RowsOfOrdersDocuments.Where(x => x.Id == req.Id)
                                             join d in context.OrdersDocuments on r.OrderDocumentId equals d.Id
                                             select d;

        tasks.Add(q.ExecuteUpdateAsync(set => set.SetProperty(p => p.LastAtUpdatedUTC, DateTime.UtcNow).SetProperty(p => p.Version, Guid.NewGuid())));

        DateTime dtu = DateTime.UtcNow;

        await context.OrdersDocuments
                .Where(x => x.Id == req.OrderDocumentId)
                .ExecuteUpdateAsync(set => set.SetProperty(p => p.LastAtUpdatedUTC, dtu));

        if (req.Id < 1)
        {
            req.Version = Guid.NewGuid();
            await context.AddAsync(req);
            await context.SaveChangesAsync();
            res.AddSuccess("Товар добавлен к заказу");
            res.Response = req.Id;
            return res;
        }
        else
        {
            RowOfOrderDocumentModelDB rowDb = await context.RowsOfOrdersDocuments.FirstAsync(x => x.Id == req.Id);
            if (rowDb.Version != req.Version)
            {
                await transaction.RollbackAsync();
                msg = "Строка документа была уже кем-то изменена";
                loggerRepo.LogError($"{msg}: {JsonConvert.SerializeObject(req, Formatting.Indented, GlobalStaticConstants.JsonSerializerSettings)}");
                res.AddError($"{msg}. Обновите документ (F5), что бы получить актуальные данные");
                return res;
            }

            int _delta = rowDb.Quantity > req.Quantity
                               ? rowDb.Quantity - req.Quantity
                               : rowDb.Quantity - req.Quantity;
            if (_delta == 0)
                res.AddInfo("Количество не изменилось");
            else
            {
                regOfferAv.Quantity += _delta;
                context.Update(regOfferAv);
            }

            tasks.Add(Task.Run(async () => res.Response = await context.RowsOfOrdersDocuments
                       .Where(x => x.Id == req.Id)
                       .ExecuteUpdateAsync(set => set
                       .SetProperty(p => p.Quantity, req.Quantity)
                       .SetProperty(p => p.Amount, req.Amount)
                       .SetProperty(p => p.Version, Guid.NewGuid()))));
        }
        await Task.WhenAll(tasks);

        context.Update(regOfferAv);
        context.Remove(locker);
        await context.SaveChangesAsync();

        await transaction.CommitAsync();
        res.AddSuccess($"Обновление `{GetType().Name}` выполнено");
        return res;
    }

    /// <inheritdoc/>
    public async Task<TResponseModel<bool>> StatusOrderChange(StatusChangeRequestModel req)
    {
        using CommerceContext context = await commerceDbFactory.CreateDbContextAsync();
        using IDbContextTransaction transaction = context.Database.BeginTransaction(System.Data.IsolationLevel.Serializable);

        TResponseModel<bool> res = new()
        {
            Response = await context
                    .OrdersDocuments
                    .Where(x => x.HelpdeskId == req.DocumentId)
                    .ExecuteUpdateAsync(set => set.SetProperty(p => p.StatusDocument, req.Step)) != 0,
        };
        await transaction.CommitAsync();
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
        int _rubricId = await context.WarehouseDocuments
            .Where(x => x.Id == req.WarehouseDocumentId)
            .Select(x => x.WarehouseId)
            .FirstAsync();

        if (_rubricId == 0)
        {
            res.AddError($"В документе не указан склад");
            return res;
        }

        using IDbContextTransaction transaction = context.Database.BeginTransaction(System.Data.IsolationLevel.Serializable);
        LockOffersAvailabilityModelDB locker = new()
        {
            LockerName = nameof(OfferAvailabilityModelDB),
            LockerId = req.OfferId,
            RubricId = _rubricId,
        };
        try
        {
            await context.AddAsync(locker);
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
            .FirstOrDefaultAsync(x => x.OfferId == req.OfferId && x.WarehouseId == _rubricId);

        if (regOfferAv is null)
        {
            regOfferAv = new()
            {
                OfferId = req.OfferId,
                GoodsId = req.GoodsId,
                WarehouseId = _rubricId,
            };

            await context.AddAsync(regOfferAv);
            await context.SaveChangesAsync();
        }
        List<Task> tasks = [];
        if (regOfferAv.GoodsId != req.GoodsId)
            tasks.Add(context.OffersAvailability.Where(x => x.Id == regOfferAv.Id).ExecuteUpdateAsync(set => set.SetProperty(p => p.GoodsId, req.GoodsId)));

        IQueryable<WarehouseDocumentModelDB> q = from r in context.RowsOfWarehouseDocuments.Where(x => x.Id == req.Id)
                                                 join d in context.WarehouseDocuments on r.WarehouseDocumentId equals d.Id
                                                 select d;

        tasks.Add(q.ExecuteUpdateAsync(set => set.SetProperty(p => p.LastAtUpdatedUTC, DateTime.UtcNow).SetProperty(p => p.Version, Guid.NewGuid())));

        if (req.Id < 1)
        {
            req.Version = Guid.NewGuid();
            regOfferAv.Quantity += req.Quantity;
            await context.AddAsync(req);
            await context.SaveChangesAsync();
            res.AddSuccess("Товар добавлен к документу");
            res.Response = req.Id;
        }
        else
        {
            RowOfWarehouseDocumentModelDB rowDb = await context.RowsOfWarehouseDocuments.FirstAsync(x => x.Id == req.Id);
            if (rowDb.Version != req.Version)
            {
                await transaction.RollbackAsync();
                msg = "Строка документа была уже кем-то изменена";
                loggerRepo.LogError($"{msg}: {JsonConvert.SerializeObject(req, Formatting.Indented, GlobalStaticConstants.JsonSerializerSettings)}");
                res.AddError($"{msg}. Обновите документ (F5), что бы получить актуальные данные");
                return res;
            }

            int _delta = rowDb.Quantity > req.Quantity
                               ? rowDb.Quantity - req.Quantity
                               : rowDb.Quantity - req.Quantity;
            if (_delta == 0)
                res.AddInfo("Количество не изменилось");
            else
            {
                regOfferAv.Quantity += _delta;
                context.Update(regOfferAv);
            }

            tasks.Add(Task.Run(async () => res.Response = await context.RowsOfWarehouseDocuments
                       .Where(x => x.Id == req.Id)
                       .ExecuteUpdateAsync(set => set
                       .SetProperty(p => p.Quantity, req.Quantity).SetProperty(p => p.Version, Guid.NewGuid()))));
        }
        await Task.WhenAll(tasks);

        context.Update(regOfferAv);
        context.Remove(locker);
        await context.SaveChangesAsync();
        await transaction.CommitAsync();

        res.AddSuccess($"Обновление `{GetType().Name}` выполнено");
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
                select new { DocumentId = d.Id, d.IsDisabled, r.OfferId, r.GoodsId, r.Quantity, d.WarehouseId };

        var _offersOfDocument = await q
           .ToArrayAsync();

        if (_offersOfDocument.Length == 0)
        {
            res.AddWarning($"Данные не найдены. Метод удаления [{nameof(RowsForWarehouseDocumentDelete)}] не выполнил запрос.");
            return res;
        }
        LockOffersAvailabilityModelDB[] offersLocked = _offersOfDocument
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

        int[] _offersIds = [.. _offersOfDocument.Select(x => x.OfferId)];
        List<OfferAvailabilityModelDB> registersOffersDb = await context.OffersAvailability
           .Where(x => _offersIds.Any(y => y == x.OfferId))
           .ToListAsync();

        int[] documents_ids = [.. _offersOfDocument.Select(x => x.DocumentId).Distinct()];
        List<Task> tasks = [.. documents_ids.Select(doc_id => context.WarehouseDocuments.Where(x => x.Id == doc_id).ExecuteUpdateAsync(set => set.SetProperty(p => p.Version, Guid.NewGuid()).SetProperty(p => p.LastAtUpdatedUTC, DateTime.UtcNow)))];

        foreach (OfferAvailabilityModelDB rowEl in registersOffersDb)
        {
            var offerDoc = _offersOfDocument.First(x => x.OfferId == rowEl.OfferId);
            rowEl.Quantity -= offerDoc.Quantity;
        }
        context.UpdateRange(registersOffersDb);

        if (offersLocked.Length != 0)
            context.RemoveRange(offersLocked);

        tasks.Add(context.SaveChangesAsync());
        tasks.Add(Task.Run(async () => res.Response = await context.RowsOfWarehouseDocuments.Where(x => req.Any(y => y == x.Id)).ExecuteDeleteAsync() != 0));

        await Task.WhenAll(tasks);
        await transaction.CommitAsync();

        res.AddSuccess("Команда удаления выполнена");
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
        req.DeliveryData = req.DeliveryData.ToUniversalTime();
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

        var _offersOfDocument = await context.RowsOfWarehouseDocuments
            .Where(x => x.WarehouseDocumentId == req.Id)
            .Select(x => new { x.WarehouseDocumentId, x.OfferId, x.GoodsId, x.Quantity })
            .ToArrayAsync();

        LockOffersAvailabilityModelDB[] offersLocked = _offersOfDocument.Length == 0
            ? []
            : _offersOfDocument.Select(x => new LockOffersAvailabilityModelDB() { LockerName = nameof(OfferAvailabilityModelDB), LockerId = x.OfferId, RubricId = req.WarehouseId }).ToArray();

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

        WarehouseDocumentModelDB whDb = await context.WarehouseDocuments
            .FirstAsync(x => x.Id == req.Id);

        if (whDb.Version != req.Version)
        {
            msg = $"Документ #{whDb.Id} был кем-то изменён (version concurent)";
            loggerRepo.LogError($"{msg}: {JsonConvert.SerializeObject(req, Formatting.Indented, GlobalStaticConstants.JsonSerializerSettings)}");
            res.AddError($"{msg}. Перед редактированием обновите страницу (F5), что бы загрузить актуальную версию объекта");
            await transaction.RollbackAsync();
            return res;
        }

        List<Task> _tasks = [];
        int[] _offersIds = [.. _offersOfDocument.Select(x => x.OfferId).Distinct()];
        List<OfferAvailabilityModelDB> registersOffersDb = await context.OffersAvailability
            .Where(x => _offersIds.Any(y => y == x.OfferId))
            .ToListAsync();

        if (whDb.IsDisabled != req.IsDisabled)
        {
            if (req.IsDisabled)
                registersOffersDb.ForEach(ro =>
                {
                    var offerDocRow = _offersOfDocument.FirstOrDefault(x => x.OfferId == ro.OfferId && x.WarehouseDocumentId == ro.WarehouseId);
                    if (offerDocRow is not null)
                    {
                        int q = ro.Quantity -= offerDocRow.Quantity;
                        _tasks.Add(context.OffersAvailability.Where(y => true).ExecuteUpdateAsync(set => set.SetProperty(p => p.Quantity, q)));
                    }
                });
            else
            {
                if (whDb.WarehouseId != req.WarehouseId)
                {

                }
            }

        }

        res.Response = await context.WarehouseDocuments
            .Where(x => x.Id == req.Id)
            .ExecuteUpdateAsync(set => set
            .SetProperty(p => p.Name, req.Name)
            .SetProperty(p => p.Description, req.Description)
            .SetProperty(p => p.DeliveryData, req.DeliveryData)
            .SetProperty(p => p.IsDisabled, req.IsDisabled)
            .SetProperty(p => p.WarehouseId, req.WarehouseId)
            .SetProperty(p => p.Version, Guid.NewGuid())
            .SetProperty(p => p.LastAtUpdatedUTC, dtu));

        res.AddSuccess(res.Response == 0 ? "Складской документ обновлён" : "Обновление складского документа не требуется");

        if (offersLocked.Length != 0)
        {
            context.RemoveRange(offersLocked);
            _tasks.Add(context.SaveChangesAsync());
        }

        if (_tasks.Count != 0)
            await Task.WhenAll(_tasks);

        await transaction.CommitAsync();
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
}