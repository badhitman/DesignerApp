////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.Extensions.Caching.Memory;
using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;
using Newtonsoft.Json;
using SharedLib;
using DbcLib;

namespace StorageService;

/// <inheritdoc/>
public class StorageServiceImpl(
    IDbContextFactory<StorageContext> cloudParametersDbFactory,
    IMemoryCache cache,
    ILogger<StorageServiceImpl> loggerRepo) : ISerializeStorage
{
#if DEBUG
    static readonly TimeSpan _ts = TimeSpan.FromSeconds(10);
#else
    static readonly TimeSpan _ts = TimeSpan.FromSeconds(60);
#endif

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
    public async Task<TResponseModel<StorageCloudParameterPayloadModel?>> ReadParameter(StorageMetadataModel req)
    {
        req.Normalize();
        string mem_key = $"{req.PropertyName}/{req.OwnerPrimaryKey}/{req.PrefixPropertyName}/{req.ApplicationName}".Replace('\\', Path.DirectorySeparatorChar).Replace('/', Path.DirectorySeparatorChar);
        TResponseModel<StorageCloudParameterPayloadModel?> res = new();
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
            TResponseModel<StorageCloudParameterPayloadModel?> _subResult = await ReadParameter(x);
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
}