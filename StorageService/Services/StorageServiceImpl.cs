////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.Extensions.Caching.Memory;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;
using MongoDB.Driver.GridFS;
using Newtonsoft.Json;
using MongoDB.Driver;
using MongoDB.Bson;
using ImageMagick;
using SharedLib;
using DbcLib;

namespace StorageService;

/// <inheritdoc/>
public class StorageServiceImpl(
    IDbContextFactory<StorageContext> cloudParametersDbFactory,
    IDbContextFactory<NLogsContext> logsDbFactory,
    IMemoryCache cache,
    IMongoDatabase mongoFs,
    IIdentityTransmission identityRepo,
    ICommerceTransmission commRepo,
    IHelpdeskTransmission HelpdeskRepo,
    WebConfigModel webConfig,
    ILogger<StorageServiceImpl> loggerRepo) : ISerializeStorage
{
#if DEBUG
    static readonly TimeSpan _ts = TimeSpan.FromSeconds(2);
#else
    static readonly TimeSpan _ts = TimeSpan.FromSeconds(10);
#endif

    #region logs
    /// <inheritdoc/>
    public async Task<TPaginationResponseModel<NLogRecordModelDB>> GoToPageForRow(TPaginationRequestModel<int> req)
    {
        if (req.PageSize < 10)
            req.PageSize = 10;

        using NLogsContext ctx = await logsDbFactory.CreateDbContextAsync();
        IQueryable<NLogRecordModelDB> q = ctx.Logs.AsQueryable();

        TPaginationResponseModel<NLogRecordModelDB> res = new()
        {
            TotalRowsCount = await q.CountAsync(),
            PageSize = req.PageSize,
            SortingDirection = req.SortingDirection,
            SortBy = req.SortBy,
        };

        if (!await q.AnyAsync(x => x.Id == req.Payload))
            return res;

        IOrderedQueryable<NLogRecordModelDB> oq = req.SortingDirection == DirectionsEnum.Up
          ? q.OrderBy(x => x.RecordTime)
          : q.OrderByDescending(x => x.RecordTime);

        res.PageNum = 0;
        while (!await oq.Skip(res.PageNum * req.PageSize).Take(req.PageSize).AnyAsync(x => x.Id == req.Payload))
            res.PageNum++;

        res.Response = [.. await oq.Skip(res.PageNum * req.PageSize).Take(req.PageSize).ToArrayAsync()];

        if (!res.Response.Any(x => x.Id == req.Payload))
            return await GoToPageForRow(req);

        return res;
    }

    /// <inheritdoc/>
    public async Task<TResponseModel<LogsMetadataResponseModel>> MetadataLogs(PeriodDatesTimesModel req)
    {
        Dictionary<string, int> LevelsAvailable = [];
        Dictionary<string, int> ApplicationsAvailable = [];
        Dictionary<string, int> ContextsPrefixesAvailable = [];
        Dictionary<string, int> LoggersAvailable = [];

        DateTime? minDate = null;
        DateTime? maxDate = null;

        IQueryable<NLogRecordModelDB> QuerySet(IQueryable<NLogRecordModelDB> q)
        {
            if (req.StartAt.HasValue)
            {
                DateTime _dt = req.StartAt.Value.SetKindUtc();
                q = q.Where(x => x.RecordTime >= _dt);
            }
            if (req.FinalOff.HasValue)
            {
                DateTime _dt = req.FinalOff.Value.SetKindUtc().Date.AddHours(23).AddMinutes(59).AddSeconds(59);
                q = q.Where(x => x.RecordTime <= _dt);
            }

            return q;
        }

        await Task.WhenAll([
                Task.Run(async () => {
                    using NLogsContext ctx = await logsDbFactory.CreateDbContextAsync();
                    minDate = await ctx.Logs.MinAsync(x => x.RecordTime);
                }),
                Task.Run(async () => {
                    using NLogsContext ctx = await logsDbFactory.CreateDbContextAsync();
                    maxDate = await ctx.Logs.MaxAsync(x => x.RecordTime);
                }),
                Task.Run(async () => {
                    using NLogsContext ctx = await logsDbFactory.CreateDbContextAsync();
                    (await QuerySet(ctx.Logs.AsQueryable()).GroupBy(x => x.RecordLevel).Select(x => new KeyValuePair<string, int>(x.Key, x.Count())).ToListAsync()).ForEach(x => LevelsAvailable.Add(x.Key, x.Value));
                }),
                Task.Run(async () => {
                    using NLogsContext ctx = await logsDbFactory.CreateDbContextAsync();
                    (await QuerySet(ctx.Logs.AsQueryable()).GroupBy(x => x.ApplicationName).Select(x => new KeyValuePair<string, int>(x.Key, x.Count())).ToListAsync()).ForEach(x => ApplicationsAvailable.Add(x.Key, x.Value));
                }),
                Task.Run(async () => {
                    using NLogsContext ctx = await logsDbFactory.CreateDbContextAsync();
                    (await QuerySet(ctx.Logs.AsQueryable()).GroupBy(x => x.ContextPrefix).Select(x => new KeyValuePair<string?, int>(x.Key, x.Count())).ToListAsync()).ForEach(x => ContextsPrefixesAvailable.Add(x.Key ?? "", x.Value));
                }),
                Task.Run(async () => {
                    using NLogsContext ctx = await logsDbFactory.CreateDbContextAsync();
                    (await QuerySet(ctx.Logs.AsQueryable()).GroupBy(x => x.Logger).Select(x => new KeyValuePair<string?, int>(x.Key, x.Count())).ToListAsync()).ForEach(x => LoggersAvailable.Add(x.Key ?? "", x.Value));
                }),
            ]);

        return new()
        {
            Response = new()
            {
                ContextsPrefixesAvailable = ContextsPrefixesAvailable,
                ApplicationsAvailable = ApplicationsAvailable,
                LoggersAvailable = LoggersAvailable,
                LevelsAvailable = LevelsAvailable,
                StartAt = minDate,
                FinalOff = maxDate,
            }
        };
    }

    /// <inheritdoc/>
    public async Task<TPaginationResponseModel<NLogRecordModelDB>> LogsSelect(TPaginationRequestModel<LogsSelectRequestModel> req)
    {
        if (req.PageSize < 10)
            req.PageSize = 10;

        using NLogsContext context = await logsDbFactory.CreateDbContextAsync();
        IQueryable<NLogRecordModelDB> q = context.Logs.AsQueryable();

        if (req.Payload.StartAt.HasValue)
        {
            DateTime _dt = req.Payload.StartAt.Value.SetKindUtc();
            q = q.Where(x => x.RecordTime >= _dt);
        }
        if (req.Payload.FinalOff.HasValue)
        {
            DateTime _dt = req.Payload.FinalOff.Value.SetKindUtc().Date.AddHours(23).AddMinutes(59).AddSeconds(59);
            q = q.Where(x => x.RecordTime <= _dt);
        }

        if (req.Payload.LevelsFilter is not null && req.Payload.LevelsFilter.Length != 0)
            q = q.Where(x => req.Payload.LevelsFilter.Contains(x.RecordLevel));

        if (req.Payload.LoggersFilter is not null && req.Payload.LoggersFilter.Length != 0)
            q = q.Where(x => req.Payload.LoggersFilter.Contains(x.Logger));

        if (req.Payload.ContextsPrefixesFilter is not null && req.Payload.ContextsPrefixesFilter.Length != 0)
            q = q.Where(x => req.Payload.ContextsPrefixesFilter.Contains(x.ContextPrefix));

        if (req.Payload.ApplicationsFilter is not null && req.Payload.ApplicationsFilter.Length != 0)
            q = q.Where(x => req.Payload.ApplicationsFilter.Contains(x.ApplicationName));

        IOrderedQueryable<NLogRecordModelDB> oq = req.SortingDirection == DirectionsEnum.Up
          ? q.OrderBy(x => x.RecordTime)
          : q.OrderByDescending(x => x.RecordTime);

        int trc = await q.CountAsync();

        return new()
        {
            PageNum = req.PageNum,
            PageSize = req.PageSize,
            SortingDirection = req.SortingDirection,
            SortBy = req.SortBy,
            TotalRowsCount = trc,
            Response = [.. await oq.Skip(req.PageNum * req.PageSize).Take(req.PageSize).ToArrayAsync()]
        };
    }
    #endregion

    #region tags
    /// <inheritdoc/>
    public async Task<TResponseModel<FilesAreaMetadataModel[]>> FilesAreaGetMetadata(FilesAreaMetadataRequestModel req)
    {
        using StorageContext context = await cloudParametersDbFactory.CreateDbContextAsync();
        IQueryable<StorageFileModelDB> q = context
            .CloudFiles
            .AsQueryable();

        if (req.ApplicationsNamesFilter is not null && req.ApplicationsNamesFilter.Length != 0)
            q = q.Where(x => req.ApplicationsNamesFilter.Contains(x.ApplicationName));

        var res = await q
            .GroupBy(x => x.ApplicationName)
            .Select(x => new
            {
                AppName = x.Key,
                CountFiles = x.Count(),
                SummSize = x.Sum(y => y.FileLength)
            })
            .ToArrayAsync();

        return new()
        {
            Response =
            [.. res
            .Select(x => new FilesAreaMetadataModel()
            {
                ApplicationName = x.AppName,
                CountFiles = x.CountFiles,
                SizeFilesSum = x.SummSize
            })]
        };
    }

    /// <inheritdoc/>
    public async Task<TPaginationResponseModel<StorageFileModelDB>> FilesSelect(TPaginationRequestModel<SelectMetadataRequestModel> req)
    {
        using StorageContext context = await cloudParametersDbFactory.CreateDbContextAsync();

        if (req.PageSize < 5)
            req.PageSize = 5;

        IQueryable<StorageFileModelDB> q = context
            .CloudFiles
            .AsQueryable();

        if (req.Payload.ApplicationsNames is not null && req.Payload.ApplicationsNames.Length != 0)
            q = q.Where(x => req.Payload.ApplicationsNames.Any(y => y == x.ApplicationName));

        if (!string.IsNullOrWhiteSpace(req.Payload.PropertyName))
            q = q.Where(x => x.PropertyName == req.Payload.PropertyName);

        if (!string.IsNullOrWhiteSpace(req.Payload.PrefixPropertyName))
            q = q.Where(x => x.PrefixPropertyName == req.Payload.PrefixPropertyName);

        if (req.Payload.OwnerPrimaryKey.HasValue && req.Payload.OwnerPrimaryKey.Value > 0)
            q = q.Where(x => x.OwnerPrimaryKey == req.Payload.OwnerPrimaryKey.Value);

        if (!string.IsNullOrWhiteSpace(req.Payload.SearchQuery))
            q = q.Where(x => x.NormalizedFileNameUpper!.Contains(req.Payload.SearchQuery.ToUpper()));

        IQueryable<StorageFileModelDB> oq = req.SortingDirection == DirectionsEnum.Up
          ? q.OrderBy(x => x.CreatedAt).Skip(req.PageNum * req.PageSize).Take(req.PageSize)
          : q.OrderByDescending(x => x.CreatedAt).Skip(req.PageNum * req.PageSize).Take(req.PageSize);

        int trc = await q.CountAsync();
        TPaginationResponseModel<StorageFileModelDB> res = new()
        {
            PageNum = req.PageNum,
            PageSize = req.PageSize,
            SortingDirection = req.SortingDirection,
            SortBy = req.SortBy,
            TotalRowsCount = trc,
            Response = await oq.ToListAsync(),
        };
        return res;
    }

    /// <inheritdoc/>
    public async Task<TResponseModel<FileContentModel>> ReadFile(TAuthRequestModel<RequestFileReadModel> req)
    {
        TResponseModel<FileContentModel> res = new();
        using StorageContext context = await cloudParametersDbFactory.CreateDbContextAsync();
        StorageFileModelDB? file_db = await context
            .CloudFiles
            .Include(x => x.AccessRules)
            .FirstOrDefaultAsync(x => x.Id == req.Payload.FileId);

        if (file_db is null)
        {
            res.AddError($"Файл #{req.Payload} не найден");
            return res;
        }

        // если правил для файла не установлено или вызывающий является владельцем (тот кто его загрузил) файла
        bool allowed = file_db.AccessRules is null || file_db.AccessRules.Count == 0 || (!string.IsNullOrEmpty(req.SenderActionUserId) && file_db.AuthorIdentityId == req.SenderActionUserId);

        string[] abs_rules = ["*", "all", "any"];
        // правило: доступ любому авторизованному пользователю
        allowed = allowed ||
            (!string.IsNullOrWhiteSpace(req.SenderActionUserId) && file_db.AccessRules?.Any(x => x.AccessRuleType == FileAccessRulesTypesEnum.User && (x.Option == req.SenderActionUserId || abs_rules.Contains(x.Option.Trim().ToLower()))) == true);

        // проверка токена прямого доступа к файлу
        allowed = allowed || (!string.IsNullOrWhiteSpace(req.Payload.TokenAccess) && file_db.AccessRules?.Any(x => x.AccessRuleType == FileAccessRulesTypesEnum.Token && x.Option == req.SenderActionUserId) == true);

        UserInfoModel? currentUser = null;
        if (!allowed && !string.IsNullOrWhiteSpace(req.SenderActionUserId))
        {
            TResponseModel<UserInfoModel[]> findUserRes = await identityRepo.GetUsersIdentity([req.SenderActionUserId]);
            currentUser = findUserRes.Response?.Single();
            if (currentUser is null)
            {
                res.AddError($"Пользователь #{req.SenderActionUserId} не найден");
                return res;
            }
            allowed = currentUser.IsAdmin;
        }

        if (!allowed)
        {
            List<string>? issues_rules = file_db
                        .AccessRules?
                        .Where(x => x.AccessRuleType == FileAccessRulesTypesEnum.Issue)
                        .Select(x => x.Option)
                        .ToList();

            if (issues_rules is not null && issues_rules.Count != 0)
            {
                List<int> issues_ids = [];
                issues_rules.ForEach(x => { if (int.TryParse(x, out int issue_id)) { issues_ids.Add(issue_id); } });
                if (issues_ids.Count > 0)
                {
                    TAuthRequestModel<IssuesReadRequestModel> reqIssues = new()
                    {
                        SenderActionUserId = req.SenderActionUserId,
                        Payload = new()
                        {
                            IssuesIds = [.. issues_ids],
                            IncludeSubscribersOnly = false,
                        }
                    };
                    TResponseModel<IssueHelpdeskModelDB[]> findIssues = await HelpdeskRepo.IssuesRead(reqIssues);
                    allowed = findIssues.Success() &&
                        findIssues.Response?.Any(x => x.AuthorIdentityUserId == req.SenderActionUserId || x.ExecutorIdentityUserId == req.SenderActionUserId || x.Subscribers?.Any(y => y.UserId == req.SenderActionUserId) == true) == true;
                }
            }
        }

        if (!allowed)
        {
            res.AddError($"Файл #{req.Payload} не прочитан");
            return res;
        }

        using MemoryStream stream = new();
        GridFSBucket gridFS = new(mongoFs);
        await gridFS.DownloadToStreamAsync(new ObjectId(file_db.PointId), stream);

        res.Response = new()
        {
            ApplicationName = file_db.ApplicationName,
            AuthorIdentityId = file_db.AuthorIdentityId,
            FileName = file_db.FileName,
            PropertyName = file_db.PropertyName,
            CreatedAt = file_db.CreatedAt,
            OwnerPrimaryKey = file_db.OwnerPrimaryKey,
            PointId = file_db.PointId,
            PrefixPropertyName = file_db.PrefixPropertyName,
            Payload = stream.ToArray(),
            Id = file_db.Id,
            ContentType = file_db.ContentType,
        };

        return res;
    }

    /// <inheritdoc/>
    public async Task<TResponseModel<StorageFileModelDB>> SaveFile(TAuthRequestModel<StorageImageMetadataModel> req)
    {
        TResponseModel<StorageFileModelDB> res = new();
        GridFSBucket gridFS = new(mongoFs);
        Regex rx = new(@"\s+", RegexOptions.Compiled);
        string _file_name = rx.Replace(req.Payload.FileName.Trim(), " ");
        if (string.IsNullOrWhiteSpace(_file_name))
            _file_name = $"без имени: {DateTime.UtcNow}";

        using MemoryStream stream = new(req.Payload.Payload);
        ObjectId _uf = await gridFS.UploadFromStreamAsync(_file_name, stream);
        using StorageContext context = await cloudParametersDbFactory.CreateDbContextAsync();
        res.Response = new StorageFileModelDB()
        {
            ApplicationName = req.Payload.ApplicationName,
            AuthorIdentityId = req.Payload.AuthorUserIdentity,
            FileName = _file_name,
            NormalizedFileNameUpper = _file_name.ToUpper(),
            ContentType = req.Payload.ContentType,
            PropertyName = req.Payload.PropertyName,
            PointId = _uf.ToString(),
            CreatedAt = DateTime.UtcNow,
            OwnerPrimaryKey = req.Payload.OwnerPrimaryKey,
            PrefixPropertyName = req.Payload.PrefixPropertyName,
            ReferrerMain = req.Payload.Referrer,
            FileLength = req.Payload.Payload.Length,
        };

        await context.AddAsync(res.Response);
        await context.SaveChangesAsync();

        if (GlobalTools.IsImageFile(_file_name))
        {
            using MagickImage image = new(req.Payload.Payload);
            //
            string _h = $"Height:{image.Height}", _w = $"Width:{image.Width}";
            await context.AddAsync(new TagModelDB()
            {
                ApplicationName = GlobalStaticConstants.Routes.FILE_CONTROLLER_NAME,
                PropertyName = GlobalStaticConstants.Routes.METADATA_CONTROLLER_NAME,
                CreatedAt = DateTime.UtcNow,
                NormalizedTagNameUpper = _h.ToUpper(),
                TagName = _h,
                OwnerPrimaryKey = res.Response.Id,
                PrefixPropertyName = GlobalStaticConstants.Routes.DEFAULT_CONTROLLER_NAME,
            });
            await context.AddAsync(new TagModelDB()
            {
                ApplicationName = GlobalStaticConstants.Routes.FILE_CONTROLLER_NAME,
                PropertyName = GlobalStaticConstants.Routes.METADATA_CONTROLLER_NAME,
                CreatedAt = DateTime.UtcNow,
                NormalizedTagNameUpper = _w.ToUpper(),
                TagName = _w,
                OwnerPrimaryKey = res.Response.Id,
                PrefixPropertyName = GlobalStaticConstants.Routes.DEFAULT_CONTROLLER_NAME,
            });
            await context.AddAsync(new TagModelDB()
            {
                ApplicationName = GlobalStaticConstants.Routes.FILE_CONTROLLER_NAME,
                PropertyName = GlobalStaticConstants.Routes.METADATA_CONTROLLER_NAME,
                CreatedAt = DateTime.UtcNow,
                NormalizedTagNameUpper = nameof(GlobalTools.IsImageFile).ToUpper(),
                TagName = nameof(GlobalTools.IsImageFile),
                OwnerPrimaryKey = res.Response.Id,
                PrefixPropertyName = GlobalStaticConstants.Routes.DEFAULT_CONTROLLER_NAME,
            });
        }

        if (req.Payload.OwnerPrimaryKey.HasValue && req.Payload.OwnerPrimaryKey.Value > 0)
        {
            PulseRequestModel reqPulse;
            string msg;
            switch (req.Payload.ApplicationName)
            {
                case GlobalStaticConstants.Routes.ORDER_CONTROLLER_NAME:
                    TResponseModel<OrderDocumentModelDB[]> get_order = await commRepo.OrdersRead(new() { Payload = [req.Payload.OwnerPrimaryKey.Value], SenderActionUserId = req.SenderActionUserId });
                    if (!get_order.Success() || get_order.Response is null)
                        res.AddRangeMessages(get_order.Messages);
                    else
                    {
                        OrderDocumentModelDB orderDb = get_order.Response.Single();
                        if (orderDb.HelpdeskId.HasValue && orderDb.HelpdeskId.Value > 0)
                        {
                            msg = $"В <a href=\"{webConfig.ClearBaseUri}/issue-card/{orderDb.HelpdeskId.Value}\">заказ #{orderDb.Id}</a> добавлен файл '<u>{_file_name}</u>' {GlobalTools.SizeDataAsString(req.Payload.Payload.Length)}";
                            loggerRepo.LogInformation($"{msg} [{nameof(res.Response.PointId)}:{_uf}]");
                            reqPulse = new()
                            {
                                Payload = new()
                                {
                                    Payload = new()
                                    {
                                        Description = msg,
                                        IssueId = orderDb.HelpdeskId.Value,
                                        PulseType = PulseIssuesTypesEnum.Files,
                                        Tag = GlobalStaticConstants.Routes.ADD_ACTION_NAME
                                    },
                                    SenderActionUserId = GlobalStaticConstants.Roles.System,
                                }
                            };

                            await HelpdeskRepo.PulsePush(reqPulse, false);
                        }
                    }
                    break;
                case GlobalStaticConstants.Routes.ISSUE_CONTROLLER_NAME:
                    msg = $"В <a href=\"{webConfig.ClearBaseUri}/issue-card/{req.Payload.OwnerPrimaryKey.Value}\">заявку #{req.Payload.OwnerPrimaryKey.Value}</a> добавлен файл '<u>{_file_name}</u>' {GlobalTools.SizeDataAsString(req.Payload.Payload.Length)}";
                    loggerRepo.LogInformation($"{msg} [{nameof(res.Response.PointId)}:{_uf}]");
                    reqPulse = new()
                    {
                        Payload = new()
                        {
                            Payload = new()
                            {
                                Description = msg,
                                IssueId = req.Payload.OwnerPrimaryKey.Value,
                                PulseType = PulseIssuesTypesEnum.Files,
                                Tag = GlobalStaticConstants.Routes.ADD_ACTION_NAME
                            },
                            SenderActionUserId = GlobalStaticConstants.Roles.System,
                        }
                    };
                    await HelpdeskRepo.PulsePush(reqPulse, false);
                    break;
            }
        }

        return res;
    }

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> TagSet(TagSetModel req)
    {
        using StorageContext context = await cloudParametersDbFactory.CreateDbContextAsync();
        ResponseBaseModel res = new();

        IQueryable<TagModelDB> q = context
            .CloudTags
            .Where(x =>
            x.OwnerPrimaryKey == req.Id &&
            x.ApplicationName == req.ApplicationName &&
            x.NormalizedTagNameUpper == req.Name.ToUpper() &&
            x.PropertyName == req.PropertyName &&
            x.PrefixPropertyName == req.PrefixPropertyName);

        if (req.Set)
        {
            if (await q.AnyAsync())
                res.AddInfo("Тег уже установлен");
            else
            {
                await context.AddAsync(new TagModelDB()
                {
                    ApplicationName = req.ApplicationName,
                    TagName = req.Name,
                    PropertyName = req.PropertyName,
                    CreatedAt = DateTime.UtcNow,
                    NormalizedTagNameUpper = req.Name.ToUpper(),
                    PrefixPropertyName = req.PrefixPropertyName,
                    OwnerPrimaryKey = req.Id,
                });
                await context.SaveChangesAsync();
            }
        }
        else
        {
            if (q.Any())
                res.AddSuccess("Тег успешно установлен");
            else
                res.AddInfo("Тег отсутствует");
        }

        return res;
    }

    /// <inheritdoc/>
    public async Task<TPaginationResponseModel<TagModelDB>> TagsSelect(TPaginationRequestModel<SelectMetadataRequestModel> req)
    {
        if (req.PageSize < 5)
            req.PageSize = 5;
        using StorageContext context = await cloudParametersDbFactory.CreateDbContextAsync();

        IQueryable<TagModelDB> q = context
            .CloudTags
            .AsQueryable();

        if (req.Payload.ApplicationsNames is not null && req.Payload.ApplicationsNames.Length != 0)
            q = q.Where(x => req.Payload.ApplicationsNames.Any(y => y == x.ApplicationName));

        if (!string.IsNullOrWhiteSpace(req.Payload.PropertyName))
            q = q.Where(x => x.PropertyName == req.Payload.PropertyName);

        if (!string.IsNullOrWhiteSpace(req.Payload.PrefixPropertyName))
            q = q.Where(x => x.PrefixPropertyName == req.Payload.PrefixPropertyName);

        if (req.Payload.OwnerPrimaryKey.HasValue && req.Payload.OwnerPrimaryKey.Value > 0)
            q = q.Where(x => x.OwnerPrimaryKey == req.Payload.OwnerPrimaryKey.Value);

        if (!string.IsNullOrWhiteSpace(req.Payload.SearchQuery))
            q = q.Where(x => x.NormalizedTagNameUpper!.Contains(req.Payload.SearchQuery.ToUpper()));

        IQueryable<TagModelDB> oq = req.SortingDirection == DirectionsEnum.Up
          ? q.OrderBy(x => x.TagName).Skip(req.PageNum * req.PageSize).Take(req.PageSize)
          : q.OrderByDescending(x => x.TagName).Skip(req.PageNum * req.PageSize).Take(req.PageSize);

        int trc = await q.CountAsync();
        return new()
        {
            PageNum = req.PageNum,
            PageSize = req.PageSize,
            SortingDirection = req.SortingDirection,
            SortBy = req.SortBy,
            TotalRowsCount = trc,
            Response = await oq.ToListAsync(),
        };
    }
    #endregion

    #region storage parameters
    /// <inheritdoc/>
    public async Task<T?[]> Find<T>(RequestStorageBaseModel req)
    {
        req.Normalize();
        using StorageContext context = await cloudParametersDbFactory.CreateDbContextAsync();
        string _tn = typeof(T).FullName ?? throw new Exception();
        StorageCloudParameterModelDB[] _dbd = await context
            .CloudProperties
            .Where(x => x.TypeName == _tn && x.ApplicationName == req.ApplicationName && x.PropertyName == req.PropertyName)
            .ToArrayAsync();

        return _dbd.Select(x => JsonConvert.DeserializeObject<T>(x.SerializedDataJson)).ToArray();
    }

    /// <inheritdoc/>
    public async Task<T?> Read<T>(StorageMetadataModel req)
    {
        req.Normalize();
        string mem_key = $"{req.PropertyName}/{req.OwnerPrimaryKey}/{req.PrefixPropertyName}/{req.ApplicationName}".Replace('\\', Path.DirectorySeparatorChar).Replace('/', Path.DirectorySeparatorChar);
        if (cache.TryGetValue(mem_key, out T? sd))
            return sd;

        using StorageContext context = await cloudParametersDbFactory.CreateDbContextAsync();
        string _tn = typeof(T).FullName ?? throw new Exception();

        StorageCloudParameterModelDB? pdb = await context
            .CloudProperties
            .Where(x => x.TypeName == _tn && x.OwnerPrimaryKey == req.OwnerPrimaryKey && x.PrefixPropertyName == req.PrefixPropertyName && x.ApplicationName == req.ApplicationName)
            .OrderByDescending(x => x.CreatedAt)
            .FirstOrDefaultAsync(x => x.PropertyName == req.PropertyName);

        if (pdb is null)
            return default;

        try
        {
            T? rawData = JsonConvert.DeserializeObject<T>(pdb.SerializedDataJson);
            cache.Set(mem_key, rawData, new MemoryCacheEntryOptions().SetAbsoluteExpiration(_ts));
            return rawData;
        }
        catch (Exception ex)
        {
            loggerRepo.LogError(ex, $"Ошибка де-сериализации [{typeof(T).FullName}] из: {pdb.SerializedDataJson}");
            return default;
        }
    }

    /// <inheritdoc/>
    public async Task Save<T>(T obj, StorageMetadataModel set, bool trimHistory = false)
    {
        if (obj is null)
            throw new ArgumentNullException(nameof(obj));
        set.Normalize();
        StorageCloudParameterModelDB _set = new()
        {
            ApplicationName = set.ApplicationName,
            PropertyName = set.PropertyName,
            TypeName = typeof(T).FullName ?? throw new Exception(),
            SerializedDataJson = JsonConvert.SerializeObject(obj),
            OwnerPrimaryKey = set.OwnerPrimaryKey,
            PrefixPropertyName = set.PrefixPropertyName,
        };
        ResponseBaseModel res = await FlushParameter(_set, trimHistory);
        if (res.Success())
        {
            string mem_key = $"{set.PropertyName}/{set.OwnerPrimaryKey}/{set.PrefixPropertyName}/{set.ApplicationName}".Replace('\\', Path.DirectorySeparatorChar).Replace('/', Path.DirectorySeparatorChar);
            cache.Set(mem_key, obj, new MemoryCacheEntryOptions().SetAbsoluteExpiration(_ts));
        }
    }

    /// <inheritdoc/>
    public async Task<TResponseModel<int?>> FlushParameter(StorageCloudParameterModelDB _set, bool trimHistory = false)
    {
        using StorageContext context = await cloudParametersDbFactory.CreateDbContextAsync();
        TResponseModel<int?> res = new();
        _set.Id = 0;
        await context.AddAsync(_set);
        bool success;
        _set.Normalize();
        Random rnd = new();
        for (int i = 0; i < 5; i++)
        {
            success = false;
            try
            {
                await context.SaveChangesAsync();
                string mem_key = $"{_set.PropertyName}/{_set.OwnerPrimaryKey}/{_set.PrefixPropertyName}/{_set.ApplicationName}".Replace('\\', Path.DirectorySeparatorChar).Replace('/', Path.DirectorySeparatorChar);
                cache.Remove(mem_key);
                success = true;
                res.AddSuccess($"Данные успешно сохранены{(i > 0 ? $" (на попытке [{i}])" : "")}: {_set.ApplicationName}/{_set.PropertyName}".Replace('\\', Path.DirectorySeparatorChar).Replace('/', Path.DirectorySeparatorChar));
                res.Response = _set.Id;
            }
            catch (Exception ex)
            {
                res.AddInfo($"Попытка записи [{i}]: {ex.Message}");
                _set.CreatedAt = DateTime.UtcNow;
                await Task.Delay(TimeSpan.FromMilliseconds(rnd.Next(100, 300)));
            }

            if (success)
                break;
        }

        IQueryable<StorageCloudParameterModelDB> qf = context
                 .CloudProperties
                 .Where(x => x.TypeName == _set.TypeName && x.ApplicationName == _set.ApplicationName && x.PropertyName == _set.PropertyName && x.OwnerPrimaryKey == _set.OwnerPrimaryKey && x.PrefixPropertyName == _set.PrefixPropertyName)
                 .AsQueryable();

        if (trimHistory)
        {
            await qf
                .Where(x => x.Id != _set.Id)
                .ExecuteDeleteAsync();
        }
        else if (await qf.CountAsync() > 50)
        {
            for (int i = 0; i < 5; i++)
            {
                success = false;
                try
                {
                    await qf
                        .OrderBy(x => x.CreatedAt)
                        .Take(50)
                        .ExecuteDeleteAsync();
                    res.AddSuccess($"Ротация успешно выполнена на попытке [{i}]");
                    success = true;
                }
                catch (Exception ex)
                {
                    res.AddInfo($"Попытка записи [{i}]: {ex.Message}");
                    await Task.Delay(TimeSpan.FromMilliseconds(rnd.Next(100, 300)));
                }

                if (success)
                    break;
            }
        }

        return res;
    }

    /// <inheritdoc/>
    public async Task<TResponseModel<StorageCloudParameterPayloadModel>> ReadParameter(StorageMetadataModel req)
    {
        req.Normalize();
        string mem_key = $"{req.PropertyName}/{req.OwnerPrimaryKey}/{req.PrefixPropertyName}/{req.ApplicationName}".Replace('\\', Path.DirectorySeparatorChar).Replace('/', Path.DirectorySeparatorChar);
        TResponseModel<StorageCloudParameterPayloadModel> res = new();
        if (cache.TryGetValue(mem_key, out StorageCloudParameterPayloadModel? sd))
        {
            res.Response = sd;
            return res;
        }
        string msg;
        using StorageContext context = await cloudParametersDbFactory.CreateDbContextAsync();
        StorageCloudParameterModelDB? parameter_db = await context
            .CloudProperties
            .OrderByDescending(x => x.CreatedAt)
            .FirstOrDefaultAsync(x =>
            x.OwnerPrimaryKey == req.OwnerPrimaryKey &&
            x.PropertyName == req.PropertyName &&
            x.ApplicationName == req.ApplicationName &&
            x.PrefixPropertyName == req.PrefixPropertyName);

        if (parameter_db is not null)
        {
            res.Response = new StorageCloudParameterPayloadModel()
            {
                ApplicationName = parameter_db.ApplicationName,
                PropertyName = parameter_db.PropertyName,
                OwnerPrimaryKey = parameter_db.OwnerPrimaryKey,
                PrefixPropertyName = parameter_db.PrefixPropertyName,
                TypeName = parameter_db.TypeName,
                SerializedDataJson = parameter_db.SerializedDataJson,
            };
            msg = $"Параметр `{req}` прочитан";
            res.AddInfo(msg);
        }
        else
        {
            msg = $"Параметр не найден: `{req}`";
            res.AddWarning(msg);
        }

        cache.Set(mem_key, res.Response, new MemoryCacheEntryOptions().SetAbsoluteExpiration(_ts));

        return res;
    }

    /// <inheritdoc/>
    public async Task<TResponseModel<List<StorageCloudParameterPayloadModel>>> ReadParameters(StorageMetadataModel[] req)
    {
        BlockingCollection<StorageCloudParameterPayloadModel> res = [];
        BlockingCollection<ResultMessage> _messages = [];
        await Task.WhenAll(req.Select(x => Task.Run(async () =>
        {
            x.Normalize();
            TResponseModel<StorageCloudParameterPayloadModel> _subResult = await ReadParameter(x);
            if (_subResult.Success() && _subResult.Response is not null)
                res.Add(_subResult.Response);
            if (_subResult.Messages.Count != 0)
                _subResult.Messages.ForEach(m => _messages.Add(m));
        })));

        return new TResponseModel<List<StorageCloudParameterPayloadModel>>()
        {
            Response = [.. res],
            Messages = [.. _messages],
        };
    }

    /// <inheritdoc/>
    public async Task<TResponseModel<FoundParameterModel[]?>> Find(RequestStorageBaseModel req)
    {
        req.Normalize();
        TResponseModel<FoundParameterModel[]?> res = new();
        using StorageContext context = await cloudParametersDbFactory.CreateDbContextAsync();
        StorageCloudParameterModelDB[] prop_db = await context
            .CloudProperties
            .Where(x => req.PropertyName == x.PropertyName && req.ApplicationName == x.ApplicationName)
            .ToArrayAsync();

        res.Response = prop_db
            .Select(x => new FoundParameterModel()
            {
                SerializedDataJson = JsonConvert.SerializeObject(x),
                CreatedAt = DateTime.UtcNow,
                OwnerPrimaryKey = x.OwnerPrimaryKey,
                PrefixPropertyName = x.PrefixPropertyName,
            })
            .ToArray();

        return res;
    }
    #endregion
}