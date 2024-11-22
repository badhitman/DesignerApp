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
public partial class CommerceImplementService : ICommerceService
{
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
                    {
                        if (registerOffer.Quantity < rowOfDocument.Quantity)
                        {
                            msg = $"Количество сторно [offer: #{rowOfDocument.OfferId} '{rowOfDocument.Offer?.Name}'] больше баланса в БД: ";
                            loggerRepo.LogError($"{msg}{JsonConvert.SerializeObject(req, Formatting.Indented, GlobalStaticConstants.JsonSerializerSettings)}");
                            res.AddError($"{msg}. Баланс не может быть отрицательным");
                            return;
                        }

                        _tasks.Add(context.OffersAvailability.Where(y => y.Id == registerOffer.Id).ExecuteUpdateAsync(set => set.SetProperty(p => p.Quantity, registerOffer.Quantity - rowOfDocument.Quantity)));
                    }
                    else
                    {
                        msg = $"Количество сторно [offer: #{rowOfDocument.OfferId} '{rowOfDocument.Offer?.Name}'] не может быть списано (остаток 0): ";
                        loggerRepo.LogError($"{msg}{JsonConvert.SerializeObject(req, Formatting.Indented, GlobalStaticConstants.JsonSerializerSettings)}");
                        res.AddError($"{msg}. Баланс не может быть отрицательным");
                        return;
                    }
                });
                if (!res.Success())
                {
                    await transaction.RollbackAsync();
                    msg = $"Не удалось выполнить обновить складской документ: ";
                    loggerRepo.LogError($"{msg}{JsonConvert.SerializeObject(req, Formatting.Indented, GlobalStaticConstants.JsonSerializerSettings)}");
                    res.AddError(msg);
                    return res;
                }
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

        if (req.Payload.DisabledOnly.HasValue)
            q = q.Where(x => x.IsDisabled == req.Payload.DisabledOnly.Value);

        if (!string.IsNullOrWhiteSpace(req.Payload.SearchQuery))
            q = q.Where(x => x.NormalizedUpperName.Contains(req.Payload.SearchQuery.ToUpper()));

        if (req.Payload.OfferFilter.HasValue && req.Payload.OfferFilter.Value != 0)
            q = q.Where(x => context.RowsOfWarehouseDocuments.Any(y => y.WarehouseDocumentId == x.Id && y.OfferId == req.Payload.OfferFilter));

        if (req.Payload.GoodsFilter.HasValue && req.Payload.GoodsFilter.Value != 0)
            q = q.Where(x => context.RowsOfWarehouseDocuments.Any(y => y.WarehouseDocumentId == x.Id && y.GoodsId == req.Payload.GoodsFilter));

        if (req.Payload.AfterDateUpdate is not null)
            q = q.Where(x => x.LastAtUpdatedUTC >= req.Payload.AfterDateUpdate || (x.LastAtUpdatedUTC == DateTime.MinValue && x.CreatedAtUTC >= req.Payload.AfterDateUpdate));

        if (req.Payload.AfterDeliveryDate is not null)
            q = q.Where(x => x.DeliveryDate >= req.Payload.AfterDeliveryDate || (x.DeliveryDate == DateTime.MinValue && x.DeliveryDate >= req.Payload.AfterDeliveryDate));


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

        if (req.Payload.MinQuantity.HasValue)
            q = q.Where(x => x.Quantity >= req.Payload.MinQuantity);

        if (req.Payload.OfferFilter is not null && req.Payload.OfferFilter.Length != 0)
            q = q.Where(x => req.Payload.OfferFilter.Any(y => y == x.OfferId));

        if (req.Payload.GoodsFilter is not null && req.Payload.GoodsFilter.Length != 0)
            q = q.Where(x => req.Payload.GoodsFilter.Any(y => y == x.GoodsId));

        if (req.Payload.WarehouseId > 0)
            q = q.Where(x => req.Payload.WarehouseId == x.WarehouseId);

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