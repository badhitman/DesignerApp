////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SharedLib;
using DbcLib;

namespace CommerceService;

/// <summary>
/// Attendance
/// </summary>
public partial class CommerceImplementService : ICommerceService
{
    /// <inheritdoc/>
    public async Task<ResponseBaseModel> OrderAttendance(TAuthRequestModel<int> req)
    {
        TResponseModel<UserInfoModel[]> actorRes = await webTransmissionRepo.GetUsersIdentity([req.SenderActionUserId]);
        if (!actorRes.Success() || actorRes.Response is null || actorRes.Response.Length == 0)
        {
            ResponseBaseModel res = new();
            res.AddRangeMessages(actorRes.Messages);
            return res;
        }
        UserInfoModel actor = actorRes.Response[0];

        using CommerceContext context = await commerceDbFactory.CreateDbContextAsync();

        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public async Task<TResponseModel<OrderAttendanceModelDB[]>> OrdersAttendancesByIssuesGet(OrdersByIssuesSelectRequestModel req)
    {
        if (req.IssueIds.Length == 0)
            return new()
            {
                Response = [],
                Messages = [new() { TypeMessage = ResultTypesEnum.Error, Text = "Запрос не может быть пустым" }]
            };

        using CommerceContext context = await commerceDbFactory.CreateDbContextAsync();
        IQueryable<OrderAttendanceModelDB> q = context
            .OrdersAttendances
            .Where(x => req.IssueIds.Any(y => y == x.HelpdeskId))
            .AsQueryable();

        Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<OrderAttendanceModelDB, NomenclatureModelDB?> inc_query = q
            .Include(x => x.Organization)
            .Include(x => x.Offer!)
            .Include(x => x.Nomenclature);

        return new()
        {
            Response = req.IncludeExternalData
            ? [.. await inc_query.ToArrayAsync()]
            : [.. await q.ToArrayAsync()],
        };
    }

    /// <inheritdoc/>
    public async Task<TResponseModel<bool>> StatusesOrdersAttendancesChangeByHelpdeskDocumentId(TAuthRequestModel<StatusChangeRequestModel> req)
    {
        TResponseModel<bool> res = new();
        TResponseModel<UserInfoModel[]> actorRes = await webTransmissionRepo.GetUsersIdentity([req.SenderActionUserId]);
        if (!actorRes.Success() || actorRes.Response is null || actorRes.Response.Length == 0)
        {
            res.AddRangeMessages(actorRes.Messages);
            return res;
        }
        UserInfoModel actor = actorRes.Response[0];
        string msg;
        using CommerceContext context = await commerceDbFactory.CreateDbContextAsync();
        List<OrderAttendanceModelDB> ordersDb = await context
            .OrdersAttendances
            .Where(x => x.HelpdeskId == req.Payload.DocumentId && x.StatusDocument != req.Payload.Step)
            .ToListAsync();

        if (ordersDb.Count == 0)
        {
            msg = "Изменение не требуется (документы для обновления отсутствуют)";
            loggerRepo.LogInformation($"{msg}: {JsonConvert.SerializeObject(req, Formatting.Indented, GlobalStaticConstants.JsonSerializerSettings)}");
            res.AddInfo($"{msg}. Перед редактированием обновите страницу (F5), что бы загрузить актуальную версию объекта");
            return res;
        }

        LockTransactionModelDB[] offersLocked = ordersDb
           .Select(x => new LockTransactionModelDB()
           {
               LockerName = $"{nameof(OrderAttendanceModelDB)} /{x.DateExecute}: {x.StartPart}-{x.EndPart}",
               LockerId = x.OfferId,
               RubricId = x.OrganizationId
           }).ToArray();

        using IDbContextTransaction transaction = context.Database.BeginTransaction(System.Data.IsolationLevel.Serializable);
        try
        {
            await context.AddRangeAsync(offersLocked);
            await context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            msg = $"Не удалось выполнить команду блокировки БД {nameof(StatusesOrdersAttendancesChangeByHelpdeskDocumentId)}: ";
            loggerRepo.LogError(ex, $"{msg}{JsonConvert.SerializeObject(req, Formatting.Indented, GlobalStaticConstants.JsonSerializerSettings)}");
            res.AddError($"{msg}{ex.Message}");
            return res;
        }

        if (req.Payload.Step == StatusesDocumentsEnum.Canceled)
        {
            ordersDb.ForEach(x => x.StatusDocument = StatusesDocumentsEnum.Canceled);
            context.UpdateRange(ordersDb);
        }
        else
        {
            WorkSchedulesFindRequestModel get_balance_req = new()
            {
                OffersFilter = ordersDb.Select(x => x.OfferId).Distinct().ToArray(),
                ContextName = GlobalStaticConstants.Routes.ATTENDANCES_CONTROLLER_NAME,
                StartDate = ordersDb.Min(x => x.DateExecute),
                EndDate = ordersDb.Max(x => x.DateExecute),
            };
            WorkSchedulesFindResponseModel get_balance = await WorkSchedulesFind(get_balance_req, ordersDb.Select(x => x.OrganizationId).Distinct().ToArray());

            foreach (IGrouping<int, WorkScheduleModel> rec in ordersDb.GroupBy(x => x.OrganizationId))
            {
                List<WorkScheduleModel> b_crop_list = get_balance.WorksSchedulesViews
                    .Where(x => x.Organization.Id == rec.Key)
                    .ToList();

                foreach (WorkScheduleModel subNode in rec)
                {
                    int cbInd = b_crop_list.FindIndex(x => x == subNode);
                    if (cbInd < 0)
                        res.AddError($"Не хватает слота: {subNode}! Удалите или измените данную запись");
                    else if (b_crop_list[cbInd].QueueCapacity == 1)
                        b_crop_list.RemoveAt(cbInd);
                    else if (b_crop_list[cbInd].QueueCapacity > 1)
                        b_crop_list[cbInd].QueueCapacity--;
                }
            }

            if (!res.Success())
            {
                await transaction.RollbackAsync();
                msg = $"Не удалось сформировать резерв:";
                loggerRepo.LogError($"{JsonConvert.SerializeObject(req, Formatting.Indented, GlobalStaticConstants.JsonSerializerSettings)}\n{JsonConvert.SerializeObject(res, Formatting.Indented, GlobalStaticConstants.JsonSerializerSettings)}");
                res.AddError(msg);
                return res;
            }
        }

        context.RemoveRange(offersLocked);
        await context.SaveChangesAsync();
        res.Response = await context
                            .OrdersAttendances
                            .Where(x => x.HelpdeskId == req.Payload.DocumentId)
                            .ExecuteUpdateAsync(set => set
                            .SetProperty(p => p.StatusDocument, req.Payload.Step)
                            .SetProperty(p => p.LastAtUpdatedUTC, DateTime.UtcNow)
                            .SetProperty(p => p.Version, Guid.NewGuid())) != 0;

        await transaction.CommitAsync();
        res.AddSuccess("Запрос смены статуса заказа услуг выполнен успешно");

        return res;
    }

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> CreateAttendanceRecords(TAuthRequestModel<CreateAttendanceRequestModel> workSchedules)
    {
        List<WorkScheduleModel> records = workSchedules.Payload.Records;
        ResponseBaseModel res = new();
        records.ForEach(x =>
        {
            ValidateReportModel ck = GlobalTools.ValidateObject(x);
            if (!ck.IsValid)
                res.Messages.InjectException(ck.ValidationResults);
        });
        string msg, waMsg;
        if (!res.Success())
        {
            msg = $"Ошибка запроса: {res.Message()}";
            loggerRepo.LogError($"{msg}{JsonConvert.SerializeObject(workSchedules, Formatting.Indented, GlobalStaticConstants.JsonSerializerSettings)}");
            res.AddError(msg);
            return res;
        }

        TResponseModel<UserInfoModel[]> actorRes = await webTransmissionRepo.GetUsersIdentity([workSchedules.SenderActionUserId]);
        if (!actorRes.Success() || actorRes.Response is null || actorRes.Response.Length == 0)
        {
            res.AddRangeMessages(actorRes.Messages);
            return res;
        }

        UserInfoModel actor = actorRes.Response[0];

        List<OrderAttendanceModelDB> recordsForAdd = records.Select(x => new OrderAttendanceModelDB()
        {
            AuthorIdentityUserId = workSchedules.SenderActionUserId,
            ContextName = GlobalStaticConstants.Routes.ATTENDANCES_CONTROLLER_NAME,
            DateExecute = x.Date,
            StartPart = TimeOnly.FromTimeSpan(x.StartPart),
            EndPart = TimeOnly.FromTimeSpan(x.EndPart),
            CreatedAtUTC = DateTime.UtcNow,
            LastAtUpdatedUTC = DateTime.UtcNow,
            OfferId = workSchedules.Payload.Offer.Id,
            NomenclatureId = workSchedules.Payload.Offer.NomenclatureId,
            OrganizationId = x.Organization.Id,
            Version = Guid.NewGuid(),
            StatusDocument = StatusesDocumentsEnum.Created,
            Name = "Новая запись"
        })
        .ToList();

        using CommerceContext context = await commerceDbFactory.CreateDbContextAsync();
        LockTransactionModelDB[] offersLocked = recordsForAdd
            .Select(x => new LockTransactionModelDB()
            {
                LockerName = $"{nameof(OrderAttendanceModelDB)} /{x.DateExecute}: {x.StartPart}-{x.EndPart}",
                LockerId = x.OfferId,
                RubricId = x.OrganizationId
            }).ToArray();
        using IDbContextTransaction transaction = context.Database.BeginTransaction(System.Data.IsolationLevel.Serializable);

        try
        {
            await context.AddRangeAsync(offersLocked);
            await context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            msg = $"Не удалось выполнить команду блокировки БД {nameof(CreateAttendanceRecords)}: ";
            loggerRepo.LogError(ex, $"{msg}{JsonConvert.SerializeObject(workSchedules, Formatting.Indented, GlobalStaticConstants.JsonSerializerSettings)}");
            res.AddError(msg);
            return res;
        }

        WorkSchedulesFindRequestModel req = new()
        {
            OffersFilter = [workSchedules.Payload.Offer.Id],
            ContextName = GlobalStaticConstants.Routes.ATTENDANCES_CONTROLLER_NAME,
            StartDate = records.Min(x => x.Date),
            EndDate = records.Max(x => x.Date),
        };

        List<WorkScheduleModel> WorksSchedulesViews = default!;
        TResponseModel<int?> res_RubricIssueForCreateOrder = default!;
        TResponseModel<string?>? CommerceNewOrderSubjectNotification = null, CommerceNewOrderBodyNotification = null, CommerceNewOrderBodyNotificationTelegram = null;

        List<Task> tasks = [
                Task.Run(async () => { CommerceNewOrderSubjectNotification = await StorageTransmissionRepo.ReadParameter<string?>(GlobalStaticConstants.CloudStorageMetadata.CommerceNewOrderSubjectNotification); }),
                Task.Run(async () => { CommerceNewOrderBodyNotification = await StorageTransmissionRepo.ReadParameter<string?>(GlobalStaticConstants.CloudStorageMetadata.CommerceNewOrderBodyNotification); }),
                Task.Run(async () => { CommerceNewOrderBodyNotificationTelegram = await StorageTransmissionRepo.ReadParameter<string?>(GlobalStaticConstants.CloudStorageMetadata.CommerceNewOrderBodyNotificationTelegram); }),
                Task.Run(async () => { res_RubricIssueForCreateOrder = await StorageTransmissionRepo.ReadParameter<int?>(GlobalStaticConstants.CloudStorageMetadata.RubricIssueForCreateAttendanceOrder); }),
                Task.Run(async () =>
                {
                    WorkSchedulesFindResponseModel get_balance = await WorkSchedulesFind(req, recordsForAdd.Select(x => x.OrganizationId).Distinct().ToArray());
                    WorksSchedulesViews = get_balance.WorksSchedulesViews;
                })
            ];

        if (string.IsNullOrWhiteSpace(_webConf.ClearBaseUri))
        {
            tasks.Add(Task.Run(async () =>
            {
                if (string.IsNullOrWhiteSpace(_webConf.ClearBaseUri))
                {
                    TelegramBotConfigModel wc = await webTransmissionRepo.GetWebConfig();
                    _webConf.BaseUri = wc.ClearBaseUri;
                }
            }));
        }

        await Task.WhenAll(tasks);

        tasks.Clear();

        foreach (IGrouping<int, WorkScheduleModel> rec in records.GroupBy(x => x.Organization.Id))
        {
            List<WorkScheduleModel> b_crop_list = WorksSchedulesViews
                .Where(x => x.Organization.Id == rec.Key)
                .ToList();

            foreach (WorkScheduleModel subNode in rec)
            {
                int cbInd = b_crop_list.FindIndex(x => x == subNode);
                if (cbInd < 0)
                    res.AddError($"Не хватает слота: {subNode}! Удалите или измените данную запись");
                else if (b_crop_list[cbInd].QueueCapacity == 1)
                    b_crop_list.RemoveAt(cbInd);
                else if (b_crop_list[cbInd].QueueCapacity > 1)
                    b_crop_list[cbInd].QueueCapacity--;
            }
        }

        if (!res.Success())
        {
            await transaction.RollbackAsync();
            msg = $"Не удалось сформировать резерв: ";
            loggerRepo.LogError($"{msg}{JsonConvert.SerializeObject(workSchedules, Formatting.Indented, GlobalStaticConstants.JsonSerializerSettings)}");
            res.AddError(msg);
            return res;
        }

        TAuthRequestModel<UniversalUpdateRequestModel> issue_new = new()
        {
            SenderActionUserId = workSchedules.SenderActionUserId,
            Payload = new()
            {
                Name = "Услуга",
                ParentId = res_RubricIssueForCreateOrder.Response,
                Description = "Услуга",
            },
        };

        TResponseModel<int> issue = await hdRepo.IssueCreateOrUpdate(issue_new);
        if (!issue.Success())
        {
            await transaction.RollbackAsync();
            res.Messages.AddRange(issue.Messages);
            return res;
        }

        recordsForAdd.ForEach(x => x.HelpdeskId = issue.Response);

        await context.AddRangeAsync(recordsForAdd);
        await context.SaveChangesAsync();

        string subject_email = "Создана новая бронь";
        DateTime _dt = DateTime.UtcNow.GetCustomTime();
        string _dtAsString = $"{_dt.ToString("d", cultureInfo)} {_dt.ToString("t", cultureInfo)}";
        string _about_order = $"Новая бронь {_dtAsString}";

        if (CommerceNewOrderSubjectNotification?.Success() == true && !string.IsNullOrWhiteSpace(CommerceNewOrderSubjectNotification.Response))
            subject_email = CommerceNewOrderSubjectNotification.Response;

        subject_email = IHelpdeskService.ReplaceTags(subject_email, _dt, issue.Response, StatusesDocumentsEnum.Created, subject_email, _webConf.ClearBaseUri, _about_order);
        res.AddSuccess(subject_email);
        msg = $"<p>Заказ <b>'{issue_new.Payload.Name}' от [{_dtAsString}]</b> успешно создан.</p>" +
                $"<p>/<a href='{_webConf.ClearBaseUri}'>{_webConf.ClearBaseUri}</a>/</p>";
        string msg_for_tg = msg.Replace("<p>", "").Replace("</p>", "");

        waMsg = $"Заказ '{issue_new.Payload.Name}' от [{_dtAsString}] успешно создан.\n{_webConf.ClearBaseUri}";

        if (CommerceNewOrderBodyNotification?.Success() == true && !string.IsNullOrWhiteSpace(CommerceNewOrderBodyNotification.Response))
            msg = CommerceNewOrderBodyNotification.Response;
        msg = IHelpdeskService.ReplaceTags(msg, _dt, issue.Response, StatusesDocumentsEnum.Created, msg, _webConf.ClearBaseUri, _about_order);

        if (CommerceNewOrderBodyNotificationTelegram?.Success() == true && !string.IsNullOrWhiteSpace(CommerceNewOrderBodyNotificationTelegram.Response))
            msg_for_tg = CommerceNewOrderBodyNotificationTelegram.Response;
        msg_for_tg = IHelpdeskService.ReplaceTags(msg_for_tg, _dt, issue.Response, StatusesDocumentsEnum.Created, msg_for_tg, _webConf.ClearBaseUri, _about_order);

        tasks = [webTransmissionRepo.SendEmail(new() { Email = actor.Email!, Subject = subject_email, TextMessage = msg }, false)];

        if (actor.TelegramId.HasValue)
            tasks.Add(tgRepo.SendTextMessageTelegram(new() { Message = msg_for_tg, UserTelegramId = actor.TelegramId!.Value }, false));

        if (!string.IsNullOrWhiteSpace(actor.PhoneNumber) && GlobalTools.IsPhoneNumber(actor.PhoneNumber!))
        {
            tasks.Add(Task.Run(async () =>
            {
                TResponseModel<string?> CommerceNewOrderBodyNotificationWhatsapp = await StorageTransmissionRepo.ReadParameter<string?>(GlobalStaticConstants.CloudStorageMetadata.CommerceNewOrderBodyNotificationWhatsapp);
                if (CommerceNewOrderBodyNotificationWhatsapp.Success() && !string.IsNullOrWhiteSpace(CommerceNewOrderBodyNotificationWhatsapp.Response))
                    waMsg = CommerceNewOrderBodyNotificationWhatsapp.Response;

                await tgRepo.SendWappiMessage(new() { Number = actor.PhoneNumber!, Text = IHelpdeskService.ReplaceTags(waMsg, _dt, issue.Response, StatusesDocumentsEnum.Created, waMsg, _webConf.ClearBaseUri, _about_order, true) }, false);
            }));
        }

        loggerRepo.LogInformation(msg_for_tg);
        context.RemoveRange(offersLocked);

        await context.SaveChangesAsync();
        await transaction.CommitAsync();
        res.AddSuccess("Ok");
        return res;
    }

    /// <inheritdoc/>
    public async Task<WorkSchedulesFindResponseModel> WorkSchedulesFind(WorkSchedulesFindRequestModel req, int[]? organizationsFilter = null)
    {
        List<DayOfWeek> weeks = [];
        List<DateOnly> dates = [];

        for (DateOnly dt = req.StartDate; dt <= req.EndDate; dt = dt.AddDays(1))
        {
            dates.Add(dt);
            if (weeks.Count != 7 && !weeks.Contains(dt.DayOfWeek))
                weeks.Add(dt.DayOfWeek);
        }

        List<WeeklyScheduleModelDB> WeeklySchedules = default!;
        List<CalendarScheduleModelDB> CalendarsSchedules = default!;
        List<OrderAttendanceModelDB> OrdersAttendances = default!;
        List<OrganizationContractorModel> OrganizationsContracts = default!;

        await Task.WhenAll([
            Task.Run(async ()=> {
                using CommerceContext context = await commerceDbFactory.CreateDbContextAsync();
                WeeklySchedules = await context.WeeklySchedules
                    .Where(x => !x.IsDisabled && x.ContextName == req.ContextName && (x.OfferId == null || req.OffersFilter.Any(y => y == x.OfferId)) && weeks.Contains(x.Weekday))
                    .ToListAsync();
             }),
            Task.Run(async ()=> {
                using CommerceContext context = await commerceDbFactory.CreateDbContextAsync();
                CalendarsSchedules = await context.CalendarsSchedules
                    .Where(x => !x.IsDisabled && x.ContextName == req.ContextName && dates.Contains(x.DateScheduleCalendar))
                    .ToListAsync();
            }),
             Task.Run(async ()=> {
                using CommerceContext context = await commerceDbFactory.CreateDbContextAsync();

                IQueryable<OrderAttendanceModelDB> q = context
                    .OrdersAttendances
                    .Where(x => x.ContextName == req.ContextName && x.DateExecute >= req.StartDate && x.DateExecute <= req.EndDate)
                    .Where(x => req.OffersFilter.Any(y => y == x.OfferId));

                if(organizationsFilter is not null && organizationsFilter.Length != 0)
                     q = q.Where(x => organizationsFilter.Any(y => y == x.OrganizationId));

                OrdersAttendances = await q
                    .Include(x => x.Offer!)
                    .ThenInclude(x => x.Nomenclature)
                    .ToListAsync();
            }),
            Task.Run(async ()=> {
                using CommerceContext context = await commerceDbFactory.CreateDbContextAsync();

                IQueryable<OrganizationContractorModel> q = context
                    .ContractorsOrganizations
                    .Where(x => x.OfferId == null || req.OffersFilter.Any(y => y == x.OfferId));

                if(organizationsFilter is not null && organizationsFilter.Length != 0)
                     q = q.Where(x => organizationsFilter.Any(y => y == x.OrganizationId));

                OrganizationsContracts = await q
                    .Include(x => x.Offer)
                    .Include(x => x.Organization!)
                    .ToListAsync();
            })
        ]);

        return new WorkSchedulesFindResponseModel(req.StartDate, req.EndDate, WeeklySchedules, CalendarsSchedules, OrganizationsContracts, OrdersAttendances);
    }

    /// <inheritdoc/>
    public async Task<TPaginationResponseModel<WeeklyScheduleModelDB>> WeeklySchedulesSelect(TPaginationRequestModel<WorkSchedulesSelectRequestModel> req)
    {
        if (req.PageSize < 10)
            req.PageSize = 10;

        using CommerceContext context = await commerceDbFactory.CreateDbContextAsync();

        IQueryable<WeeklyScheduleModelDB> q = context
            .WeeklySchedules.Where(x => x.ContextName == req.Payload.ContextName && x.OfferId == req.Payload.OfferFilter && x.NomenclatureId == req.Payload.NomenclatureFilter)
            .AsQueryable();

        if (req.Payload.Weekdays is not null && req.Payload.Weekdays.Length != 0)
            q = q.Where(x => req.Payload.Weekdays.Any(y => y == x.Weekday));

        if (req.Payload.AfterDateUpdate is not null)
            q = q.Where(x => x.LastAtUpdatedUTC >= req.Payload.AfterDateUpdate || (x.LastAtUpdatedUTC == DateTime.MinValue && x.CreatedAtUTC >= req.Payload.AfterDateUpdate));

        IOrderedQueryable<WeeklyScheduleModelDB> oq = req.SortingDirection == VerticalDirectionsEnum.Up
           ? q.OrderBy(x => x.StartPart).ThenByDescending(x => x.LastAtUpdatedUTC)
           : q.OrderByDescending(x => x.StartPart).ThenByDescending(x => x.LastAtUpdatedUTC);

        IQueryable<WeeklyScheduleModelDB> pq = oq
            .Skip(req.PageNum * req.PageSize)
            .Take(req.PageSize);

        Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<WeeklyScheduleModelDB, NomenclatureModelDB?> inc_query = pq
            .Include(x => x.Offer)
            .Include(x => x.Nomenclature);

        return new()
        {
            PageNum = req.PageNum,
            PageSize = req.PageSize,
            SortingDirection = req.SortingDirection,
            SortBy = req.SortBy,
            TotalRowsCount = await q.CountAsync(),
            Response = req.Payload.IncludeExternalData ? [.. await inc_query.ToArrayAsync()] : [.. await pq.ToArrayAsync()]
        };
    }

    /// <inheritdoc/>
    public async Task<TResponseModel<int>> WeeklyScheduleUpdate(WeeklyScheduleModelDB req)
    {
        TResponseModel<int> res = new() { Response = 0 };
        ValidateReportModel ck = GlobalTools.ValidateObject(req);
        if (!ck.IsValid)
        {
            res.Messages.InjectException(ck.ValidationResults);
            return res;
        }

        req.Name = req.Name.Trim();
        req.Description = req.Description?.Trim();
        req.NormalizedNameUpper = req.Name.ToUpper();
        req.LastAtUpdatedUTC = DateTime.UtcNow;

        using CommerceContext context = await commerceDbFactory.CreateDbContextAsync();
        if (req.Id < 1)
        {
            req.IsDisabled = true;
            req.Id = 0;
            req.CreatedAtUTC = req.LastAtUpdatedUTC;
            context.Add(req);
            await context.SaveChangesAsync();
            res.Response = req.Id;
        }
        else
        {
            res.Response = await context.WeeklySchedules
                .Where(w => w.Id == req.Id)
                .ExecuteUpdateAsync(set => set
                .SetProperty(p => p.NormalizedNameUpper, req.NormalizedNameUpper)
                .SetProperty(p => p.LastAtUpdatedUTC, DateTime.UtcNow)
                .SetProperty(p => p.Description, req.Description)
                .SetProperty(p => p.IsDisabled, req.IsDisabled)
                .SetProperty(p => p.StartPart, req.StartPart)
                .SetProperty(p => p.EndPart, req.EndPart)
                .SetProperty(p => p.QueueCapacity, req.QueueCapacity)
                .SetProperty(p => p.Name, req.Name));
        }

        return res;
    }

    /// <inheritdoc/>
    public async Task<List<WeeklyScheduleModelDB>> WeeklySchedulesRead(int[] req)
    {
        using CommerceContext context = await commerceDbFactory.CreateDbContextAsync();

        IQueryable<WeeklyScheduleModelDB> q = context
            .WeeklySchedules
            .Where(x => req.Any(y => x.Id == y));

        return await q
            .Include(x => x.Offer!)
            .Include(x => x.Nomenclature)
            .ToListAsync();
    }

    /// <inheritdoc/>
    public async Task<TResponseModel<int>> CalendarScheduleUpdate(TAuthRequestModel<CalendarScheduleModelDB> req)
    {
        TResponseModel<int> res = new() { Response = 0 };
        ValidateReportModel ck = GlobalTools.ValidateObject(req);
        if (!ck.IsValid)
        {
            res.Messages.InjectException(ck.ValidationResults);
            return res;
        }

        req.Payload.Name = req.Payload.Name.Trim();
        req.Payload.Description = req.Payload.Description?.Trim();
        req.Payload.NormalizedNameUpper = req.Payload.Name.ToUpper();

        using CommerceContext context = await commerceDbFactory.CreateDbContextAsync();
        if (req.Payload.Id < 1)
        {
            req.Payload.IsDisabled = true;
            req.Payload.Id = 0;
            req.Payload.CreatedAtUTC = DateTime.UtcNow;
            req.Payload.LastAtUpdatedUTC = DateTime.UtcNow;
            context.Add(req.Payload);
            await context.SaveChangesAsync();
            res.Response = req.Payload.Id;
        }
        else
        {
            res.Response = await context.CalendarsSchedules
                .Where(x => x.Id == req.Payload.Id)
                .ExecuteUpdateAsync(set => set
                .SetProperty(p => p.Name, req.Payload.Name)
                .SetProperty(p => p.Description, req.Payload.Description)
                .SetProperty(p => p.IsDisabled, req.Payload.IsDisabled)
                .SetProperty(p => p.QueueCapacity, req.Payload.QueueCapacity)
                .SetProperty(p => p.EndPart, req.Payload.EndPart)
                .SetProperty(p => p.StartPart, req.Payload.StartPart)
                .SetProperty(p => p.DateScheduleCalendar, req.Payload.DateScheduleCalendar)
                .SetProperty(p => p.LastAtUpdatedUTC, DateTime.UtcNow)
                .SetProperty(p => p.NormalizedNameUpper, req.Payload.NormalizedNameUpper));
        }

        return res;
    }

    /// <inheritdoc/>
    public async Task<TPaginationResponseModel<CalendarScheduleModelDB>> CalendarSchedulesSelect(TPaginationRequestModel<WorkScheduleCalendarsSelectRequestModel> req)
    {
        if (req.PageSize < 10)
            req.PageSize = 10;

        using CommerceContext context = await commerceDbFactory.CreateDbContextAsync();

        DateOnly _dtp = DateOnly.FromDateTime(DateTime.UtcNow);

        IQueryable<CalendarScheduleModelDB> q = context
            .CalendarsSchedules
            .Where(x => x.OfferId == req.Payload.OfferFilter && x.NomenclatureId == req.Payload.NomenclatureFilter && (!req.Payload.ActualOnly || x.DateScheduleCalendar >= _dtp))
            .AsQueryable();

        if (req.Payload.AfterDateUpdate is not null)
            q = q.Where(x => x.LastAtUpdatedUTC >= req.Payload.AfterDateUpdate || (x.LastAtUpdatedUTC == DateTime.MinValue && x.CreatedAtUTC >= req.Payload.AfterDateUpdate));

        IOrderedQueryable<CalendarScheduleModelDB> oq = req.SortingDirection == VerticalDirectionsEnum.Up
           ? q.OrderBy(x => x.DateScheduleCalendar).ThenBy(x => x.StartPart).ThenByDescending(x => x.LastAtUpdatedUTC)
           : q.OrderByDescending(x => x.DateScheduleCalendar).ThenByDescending(x => x.StartPart).ThenByDescending(x => x.LastAtUpdatedUTC);

        IQueryable<CalendarScheduleModelDB> pq = oq
            .Skip(req.PageNum * req.PageSize)
            .Take(req.PageSize);

        Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<CalendarScheduleModelDB, NomenclatureModelDB?> inc_query = pq
            .Include(x => x.Offer)
            .Include(x => x.Nomenclature);

        CalendarScheduleModelDB[] res = req.Payload.IncludeExternalData
            ? await inc_query.ToArrayAsync()
            : await pq.ToArrayAsync();

        return new()
        {
            PageNum = req.PageNum,
            PageSize = req.PageSize,
            SortingDirection = req.SortingDirection,
            SortBy = req.SortBy,
            TotalRowsCount = await q.CountAsync(),
            Response = [.. res]
        };
    }

    /// <inheritdoc/>
    public async Task<List<CalendarScheduleModelDB>> CalendarSchedulesRead(int[] req)
    {
        TResponseModel<CalendarScheduleModelDB[]> res = new();

        using CommerceContext context = await commerceDbFactory.CreateDbContextAsync();

        IQueryable<CalendarScheduleModelDB> q = context
            .CalendarsSchedules
            .Where(x => req.Any(y => x.Id == y));

        return await q
            .Include(x => x.Offer!)
            .Include(x => x.Nomenclature)
            .ToListAsync();
    }
}