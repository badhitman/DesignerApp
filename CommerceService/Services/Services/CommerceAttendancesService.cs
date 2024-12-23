﻿////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;
using SharedLib;
using DbcLib;
using System.Linq;

namespace CommerceService;

/// <summary>
/// Attendance
/// </summary>
public partial class CommerceImplementService : ICommerceService
{
    /// <inheritdoc/>
    public async Task<WorkSchedulesFindResponseModel> WorkSchedulesFind(WorkSchedulesFindRequestModel req)
    {
        WorkSchedulesFindResponseModel res = new(req.StartDate, req.EndDate);
        if (res.StartDate > res.EndDate)
            return res;

        List<DayOfWeek> weeks = [];
        List<DateOnly> dates = [];

        for (DateOnly dt = req.StartDate; dt <= req.EndDate; dt = dt.AddDays(1))
        {
            dates.Add(dt);
            if (weeks.Count != 7 && !weeks.Contains(dt.DayOfWeek))
                weeks.Add(dt.DayOfWeek);
        }

        await Task.WhenAll([
            Task.Run(async ()=> {
                using CommerceContext context = await commerceDbFactory.CreateDbContextAsync();
                res.Schedules = await context.WeeklySchedules
                    .Where(x => !x.IsDisabled && x.ContextName == req.ContextName && req.OffersFilter.Any(y => y == x.OfferId) && weeks.Contains(x.Weekday))
                    .ToListAsync();
             }),
             Task.Run(async ()=> {
                using CommerceContext context = await commerceDbFactory.CreateDbContextAsync();
                res.OrdersAttendances = await context
                    .OrdersAttendances
                    .Where(x => x.ContextName == req.ContextName && x.DateExecute >= req.StartDate && x.DateExecute <= req.EndDate)
                    .Where(x => req.OffersFilter.Any(y => y == x.OfferId))
                    .Include(x => x.Offer!)
                    .ThenInclude(x => x.Nomenclature)
                    .Select(x => new OrderAnonModelDB()
                    {
                        DateExecute = x.DateExecute,
                        StartPart = x.StartPart,
                        EndPart = x.EndPart,
                        Offer = x.Offer!,
                    }).ToListAsync();
            }),
            Task.Run(async ()=> {
                using CommerceContext context = await commerceDbFactory.CreateDbContextAsync();
                res.OrganizationsContracts = await context
                    .ContractorsOrganizations
                    .Where(x => x.OfferId == null || req.OffersFilter.Any(y => y == x.OfferId))
                    .Include(x => x.Offer)
                    //.Include(x => x.Organization!)
                    //.ThenInclude(x => x.Users)
                    .ToListAsync();
            }),
            Task.Run(async ()=> {
                using CommerceContext context = await commerceDbFactory.CreateDbContextAsync();
                res.Calendars = await context.CalendarsSchedules
                    .Where(x => !x.IsDisabled && x.ContextName == req.ContextName && dates.Contains(x.DateScheduleCalendar))
                    .ToListAsync();
            })
        ]);

        return res;
    }

    /// <inheritdoc/>
    public async Task<TResponseModel<TPaginationResponseModel<WeeklyScheduleModelDB>>> WeeklySchedulesSelect(TPaginationRequestModel<WorkSchedulesSelectRequestModel> req)
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
    public async Task<TResponseModel<WeeklyScheduleModelDB[]>> WeeklySchedulesRead(int[] req)
    {
        TResponseModel<WeeklyScheduleModelDB[]> res = new();

        using CommerceContext context = await commerceDbFactory.CreateDbContextAsync();

        IQueryable<WeeklyScheduleModelDB> q = context
            .WeeklySchedules
            .Where(x => req.Any(y => x.Id == y));

        res.Response = await q
            .Include(x => x.Offer!)
            .Include(x => x.Nomenclature)
            .ToArrayAsync();

        return res;
    }

    /// <inheritdoc/>
    public async Task<TResponseModel<int>> CalendarScheduleUpdate(CalendarScheduleModelDB req)
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

        using CommerceContext context = await commerceDbFactory.CreateDbContextAsync();
        if (req.Id < 1)
        {
            req.IsDisabled = true;
            req.Id = 0;
            req.CreatedAtUTC = DateTime.UtcNow;
            req.LastAtUpdatedUTC = DateTime.UtcNow;
            context.Add(req);
            await context.SaveChangesAsync();
            res.Response = req.Id;
        }
        else
        {
            res.Response = await context.CalendarsSchedules
                .Where(x => x.Id == req.Id)
                .ExecuteUpdateAsync(set => set
                .SetProperty(p => p.Name, req.Name)
                .SetProperty(p => p.Description, req.Description)
                .SetProperty(p => p.IsDisabled, req.IsDisabled)
                .SetProperty(p => p.QueueCapacity, req.QueueCapacity)
                .SetProperty(p => p.EndPart, req.EndPart)
                .SetProperty(p => p.StartPart, req.StartPart)
                .SetProperty(p => p.DateScheduleCalendar, req.DateScheduleCalendar)
                .SetProperty(p => p.LastAtUpdatedUTC, DateTime.UtcNow)
                .SetProperty(p => p.NormalizedNameUpper, req.NormalizedNameUpper));
        }

        return res;
    }

    /// <inheritdoc/>
    public async Task<TResponseModel<TPaginationResponseModel<CalendarScheduleModelDB>>> CalendarSchedulesSelect(TPaginationRequestModel<WorkScheduleCalendarsSelectRequestModel> req)
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
            Response = new()
            {
                PageNum = req.PageNum,
                PageSize = req.PageSize,
                SortingDirection = req.SortingDirection,
                SortBy = req.SortBy,
                TotalRowsCount = await q.CountAsync(),
                Response = [..res]
            },
        };
    }

    /// <inheritdoc/>
    public async Task<TResponseModel<CalendarScheduleModelDB[]>> CalendarSchedulesRead(int[] req)
    {
        TResponseModel<CalendarScheduleModelDB[]> res = new();

        using CommerceContext context = await commerceDbFactory.CreateDbContextAsync();

        IQueryable<CalendarScheduleModelDB> q = context
            .CalendarsSchedules
            .Where(x => req.Any(y => x.Id == y));

        res.Response = await q
            .Include(x => x.Offer!)
            .Include(x => x.Nomenclature)
            .ToArrayAsync();

        return res;
    }
}